using Catty.Core.Channel;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Catty.Core.Sockets.Nio
{
    public class NioClientSocketChannel : NioSocketChannel
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NioClientSocketChannel));

        private static Socket NewSocket()
        {
            Socket socket;
            try
            {
                socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            }
            catch (Exception e)
            {
                throw new ChannelException("Failed to open a socket.", e);
            }

            bool success = false;
            try
            {
                //socket.configureBlocking(false);
                success = true;
            }
            catch (Exception e)
            {
                throw new ChannelException("Failed to enter non-blocking mode.", e);
            }
            finally
            {
                if (!success)
                {
                    try
                    {
                        socket.Close();
                    }
                    catch (Exception e)
                    {
                        //if (logger.isWarnEnabled())
                        //{
                        //    logger.warn(
                        //            "Failed to close a partially initialized socket.",
                        //            e);
                        //}
                    }
                }
            }

            return socket;
        }

        volatile IChannelFuture connectFuture;
        internal bool boundManually;

        // Does not need to be volatile as it's accessed by only one thread.
        long connectDeadlineNanos;
        volatile SocketAddress requestedRemoteAddress;

        internal volatile Timer timoutTimer;

        internal NioClientSocketChannel(
                IChannelFactory factory, IChannelPipeline pipeline,
                IChannelSink sink)
            : base(null, factory, pipeline, sink, NewSocket())
        {
            Channels.FireChannelOpen(this);
        }

        internal void Bind(IPEndPoint localEndPoint)
        {
            this.socket.Bind(localEndPoint);
        }

        public override IChannelFuture Connect(EndPoint remoteAddress)
        {
            IChannelFuture cf = Channels.Future(this);
            Connect(cf, remoteAddress);
            return cf;
        }
        
        private bool isConnecting;
        private DateTime startConnectTime;
        private EndPoint remoteEndPoint;
        // called by Sink only
        internal void Connect(IChannelFuture cf, EndPoint remoteEndPoint)
        {
            // BeginConnect
            this.remoteEndPoint = remoteEndPoint;
            this.startConnectTime = DateTime.UtcNow;
            socket.BeginConnect(remoteEndPoint, new AsyncCallback(ConnectCallback), cf);
            isConnecting = true;
            if (log.IsInfoEnabled)
                log.Info("event=BeginConnect remoteEndPoint=" + remoteEndPoint.ToString());
        }

        // callback for Socket::BeginConnect()
        internal void ConnectCallback(IAsyncResult result)
        {
            // Finish connection
            long elapsed = (long)(DateTime.UtcNow - this.startConnectTime).TotalMilliseconds;
            try
            {
                socket.EndConnect(result);

                this.StartIncommingThread();
                Channels.FireChannelConnected(this);
                IChannelFuture cf = result.AsyncState as IChannelFuture;
                cf.SetSuccess();

                if (log.IsInfoEnabled)
                    log.Info("event=EndConnect remoteEndPoint=" + remoteEndPoint.ToString() + " conclusion=success elapsed=" + elapsed);
            }
            catch (SocketException e)
            {
                if (log.IsWarnEnabled)
                    log.Warn("event=EndConnect remoteEndPoint=" + remoteEndPoint.ToString() + " conclusion=SocketException elapsed=" + elapsed + " e="+ e.ToString());

                return;
            }
        }


    }
}
