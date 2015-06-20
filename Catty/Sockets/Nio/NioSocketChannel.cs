using Catty.Core.Buffer;
using Catty.Core.Channel;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

namespace Catty.Core.Sockets.Nio
{
    // known child class: NioAcceptedSocketChannel, NioClientSocketChannel
    public class NioSocketChannel : AbstractChannel, ISocketChannel
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NioSocketChannel));

        private const int ST_OPEN = 0;
        private const int ST_BOUND = 1;
        private const int ST_CONNECTED = 2;
        private const int ST_CLOSED = -1;
        volatile int state = ST_OPEN;

        private readonly INioSocketChannelConfig config;

        internal readonly Socket socket;

        public NioSocketChannel(
                    IChannel parent, IChannelFactory factory,
                    IChannelPipeline pipeline, IChannelSink sink,
                    Socket socket)
            : base(parent, factory, pipeline, sink)
        {
            this.socket = socket;
            SetTcpKeepAlive(socket, 10 * 1000, 5 * 1000);
            config = new DefaultNioSocketChannelConfig(socket);
        }

        internal void ExecuteInIoThread(ChannelRunnableWrapper wrapper)
        {
            throw new NotImplementedException();
        }

        public override bool IsBound()
        {
            return state >= ST_BOUND;
        }

        public override bool IsConnected()
        {
            return state >= ST_CONNECTED;
        }

        internal void SetBound() {
            //assert state == ST_OPEN : "Invalid state: " + state;
            state = ST_BOUND;
        }

        public override EndPoint GetLocalAddress()
        {
            return this.socket.LocalEndPoint;
        }

        public override EndPoint GetRemoteAddress()
        {
            return this.socket.RemoteEndPoint;
        }

        public EndPoint GetLocalSocketAddress()
        {
            return this.socket.LocalEndPoint;
        }

        public EndPoint GetRemoteSocketAddress()
        {
            return this.socket.RemoteEndPoint;
        }

        public override int GetInterestOps()
        {
            throw new NotImplementedException();
        }

        public override IChannelFuture SetInterestOps(int interestOps)
        {
            throw new NotImplementedException();
        }

        internal void SetConnected()
        {
            if (state != ST_CLOSED)
            {
                state = ST_CONNECTED;
            }
        }

        public override IChannelConfig GetConfig()
        {
            return config;
        }

        #region BeginSend/EndSend

        private object sendingLock = new object();
        private bool isSending = false;
        private byte[] outputBuf = null; // pinned buf
        private int outputDataEndIndex;
        private List<IMessageEvent> preSendingQueue = new List<IMessageEvent>(); // wating line
        private List<IMessageEvent> postSendingQueue = new List<IMessageEvent>(); // events in this queue are already write into the the outputBuf and we just waiting for the EndSend() call back to finish the futures.
        internal void WriteIntoSocket(IMessageEvent eve) // only to be called by Sink
        {
            bool needStartSend = false;
            lock (this.sendingLock)
            {
                preSendingQueue.Add(eve);
                if (!isSending) // we need to start sending thread
                {
                    outputBuf = DefaultPinedBufferFactory.GetInstance().GetBuffer(500);
                    outputDataEndIndex = 0;
                    RefillOutputBuf();
                    if (outputDataEndIndex > 0)
                    {
                        needStartSend = true;
                        this.isSending = true;
                    }
                    else
                    {
                        DefaultPinedBufferFactory.GetInstance().ReleaseBuffer(outputBuf);
                        outputBuf = null;
                    }
                }
            }
            if (needStartSend)
            {
                this.socket.BeginSend(outputBuf, 0, outputDataEndIndex, SocketFlags.None, this.SendCallback, null);
            }
        }

        private void SendCallback(IAsyncResult result)
        {
            int bytesSend = 0;
            try
            {
                bytesSend = socket.EndSend(result);
            }
            catch (Exception e)
            {
                Channels.FireExceptionCaught(this, e);
                lock (this.sendingLock)
                {
                    foreach (var eve in postSendingQueue)
                    {
                        eve.GetFuture().SetFailure(e);
                    }
                }
            }
            bool needStartSend = false;
            lock (this.sendingLock)
            {
                if (bytesSend < outputDataEndIndex)
                {
                    Array.Copy(outputBuf, bytesSend, outputBuf, 0, outputDataEndIndex - bytesSend);
                    outputDataEndIndex -= bytesSend;
                }
                else
                {
                    outputDataEndIndex = 0;
                }

                {
                    foreach (var eve in postSendingQueue)
                    {
                        eve.GetFuture().SetSuccess();
                    }
                    postSendingQueue.Clear();
                }

                RefillOutputBuf();
                if (outputDataEndIndex > 0)
                {
                    needStartSend = true;
                }
                else
                {
                    DefaultPinedBufferFactory.GetInstance().ReleaseBuffer(outputBuf);
                    outputBuf = null;
                    this.isSending = false;
                }
            }
            if (needStartSend)
            {
                this.socket.BeginSend(outputBuf, 0, outputDataEndIndex, SocketFlags.None, this.SendCallback, null);
            }
        }

        private void RefillOutputBuf() // please get lock before call this
        {
            while (preSendingQueue.Count > 0 && outputDataEndIndex < outputBuf.Length)
            {
                IMessageEvent dequeue = preSendingQueue[0];
                object msg = dequeue.GetMessage();
                if (msg == null)
                {
                    dequeue.GetFuture().SetFailure(new NullReferenceException("no data or not serilized?"));
                    preSendingQueue.RemoveAt(0);
                }
                else if (msg is IByteBuf)
                {
                    IByteBuf data = (IByteBuf)msg;
                    int length = Math.Min(data.ReadableBytes, outputBuf.Length - outputDataEndIndex);
                    if (length > 0)
                    {
                        data.ReadBytes(outputBuf, outputDataEndIndex, length);
                        outputDataEndIndex += length;
                    }
                    if (data.ReadableBytes == 0) // no more data available
                    {
                        data.Release();
                        postSendingQueue.Add(dequeue);
                        preSendingQueue.RemoveAt(0);
                    }
                }
                else
                {
                    dequeue.GetFuture().SetFailure(new NullReferenceException("no data or not serilized?"));
                    preSendingQueue.RemoveAt(0);
                }
            }
        }
        #endregion

        #region BeginReceive/EndReceive
        
        private object receiveLock = new object();
        private byte[] incommingBuf;

        // this need to be called after socket connected (or accepted)
        public void StartIncommingThread()
        {
            try
            {
                this.incommingBuf = DefaultPinedBufferFactory.GetInstance().GetBuffer(100);
                // now connection is establish, we can send and receive from this socket
                socket.BeginReceive(incommingBuf, 0, incommingBuf.Length, SocketFlags.None,this.ReceiveCallback, null);
            }
            catch (SocketException ex)
            {
                if (log.IsWarnEnabled)
                {
                    log.Warn("event=SocketException", ex);
                }
            }
            catch (Exception ex)
            {
                if (log.IsWarnEnabled)
                {
                    log.Warn("event=Exception", ex);
                }
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            int bytesRead = 0;
            try
            {
                bytesRead = socket.EndReceive(result);
                if (bytesRead > 0)
                {
                    var buffer = DynamicByteBuf.GetInstance();
                    buffer.WriteBytes(incommingBuf, 0, bytesRead);
                    // note: This is in I/O callback thread. which means the first callback in handler chain is going to be I/O thread.
                    Channels.FireMessageReceived(this, buffer);
                    buffer.Release();
                    if (state > ST_OPEN)
                    {
                        socket.BeginReceive(incommingBuf, 0, incommingBuf.Length, SocketFlags.None, this.ReceiveCallback, null);
                    }
                    else
                    {
                        // socket closed during message handling loop.
                        if (log.IsInfoEnabled)
                        {
                            log.Info("event=NioSocketChannel_Closed_Local1 socket=" + this.ToString());
                        }
                        ReleaseIncommingBuf();
                    }
                }
                else
                {
                    // socket closed by remote party
                    state = ST_CLOSED;
                    Channels.FireExceptionCaught(this, new SocketException(10054)); // Socket Exception 10054 (connection reset by peer)
                    if (log.IsInfoEnabled)
                    {
                        log.Info("event=NioSocketChannel_Closed_Remote socket=" + this.ToString());
                    }
                    ReleaseIncommingBuf();
                }
            }
            catch (ObjectDisposedException ex)
            {
                // we see this happen when we call socket.Close() when Receive thread is pending.
                if (log.IsInfoEnabled)
                {
                    log.Info("event=NioSocketChannel_Closed_Local2 socket=" + this.ToString());
                }
                ReleaseIncommingBuf();
            }
            catch (Exception ex)
            {
                state = ST_CLOSED;
                if (log.IsWarnEnabled)
                {
                    log.Warn("event=NioSocketChannel_Exception socket=" + this.ToString(), ex);
                }
                ReleaseIncommingBuf();
            }
        }

        private void ReleaseIncommingBuf()
        {
            // clean up and release resource
            if (this.incommingBuf != null)
            {
                DefaultPinedBufferFactory.GetInstance().ReleaseBuffer(this.incommingBuf);
                this.incommingBuf = null;
            }
        }

        #endregion

        internal void Close(NioSocketChannel channel, IChannelFuture future)
        {
            if (state > ST_CLOSED)
            {
                state = ST_CLOSED;
                if (log != null && log.IsInfoEnabled)
                    log.Info("event=ClosingSocket localEndPoint=" + GetLocalSocketAddress() + " remoteEndPoint=" + GetRemoteSocketAddress());
                socket.Close(); // hopefully close() should will never fail :)
            }
            future.SetSuccess();
        }

        // http://stackoverflow.com/questions/169170/what-is-the-best-way-to-do-keep-alive-socket-checking-in-net
        public static void SetTcpKeepAlive(Socket socket, uint keepaliveTime, uint keepaliveInterval) // in ms, 
        {
            /* the native structure
            struct tcp_keepalive {
            ULONG onoff;
            ULONG keepalivetime;
            ULONG keepaliveinterval;
            };
            */
            // http://msdn.microsoft.com/en-us/library/windows/desktop/dd877220%28v=vs.85%29.aspx

            // marshal the equivalent of the native structure into a byte array
            uint dummy = 0;
            byte[] inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
            BitConverter.GetBytes((uint)(keepaliveTime)).CopyTo(inOptionValues, 0);
            BitConverter.GetBytes((uint)keepaliveTime).CopyTo(inOptionValues, Marshal.SizeOf(dummy));
            BitConverter.GetBytes((uint)keepaliveInterval).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);

            // write SIO_VALS to Socket IOControl
            socket.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
        }
    }
}
