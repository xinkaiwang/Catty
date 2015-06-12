using Catty.Core.Channel;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Sockets.Nio
{
    public class NioServerSocketChannel : AbstractServerChannel, IServerSocketChannel
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NioServerSocketChannel));

        public const int MAX_CONNECTIONS = 10000;

        private Socket socket;
        private Object lockObj = new object(); // lock this before modify anything in "connections" array.
        public bool isAccepting = false; // true means the accept thread is running.
        private readonly IServerSocketChannelConfig config;

        public NioServerSocketChannel(IChannelFactory factory,
            IChannelPipeline pipeline,
            IChannelSink sink)
            : base(factory, pipeline, sink)
        {
            try
            {
                socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            }
            catch (SocketException e)
            {
                throw new ChannelException(
                        "Failed to open a server socket.", e);
            }

            config = new DefaultServerSocketChannelConfig(socket);
            Channels.FireChannelOpen(this);
        }

        public IServerSocketChannelConfig GetServerSocketChannelConfig()
        {
            return config;
        }

        public override IChannelConfig GetConfig()
        {
            return GetServerSocketChannelConfig();
        }

        public override EndPoint GetLocalAddress()
        {
            return socket.LocalEndPoint;
        }

        public override EndPoint GetRemoteAddress()
        {
            return null;
        }

        public EndPoint GetLocalSocketAddress()
        {
            return socket.LocalEndPoint;
        }

        public EndPoint GetRemoteSocketAddress()
        {
            return null;
        }

        public override bool IsBound()
        {
            return IsOpen() && socket.IsBound;
        }

        protected override bool SetClosed()
        {
            return base.SetClosed();
        }

        internal void BindAndStartAccept(EndPoint localEndPoint) // only to be called by Sink
        {
            this.socket.Bind(localEndPoint);
            this.socket.Listen(MAX_CONNECTIONS); // max length of the pending connection queue
            this.isAccepting = true;
            this.socket.BeginAccept(this.AcceptCallback, null);
        }

        private void NewSocketAccepted(Socket newSocket)
        {
            IChannelSink sink = this.GetPipeline().GetSink();
            IChannelPipeline pipeline = this.GetConfig().GetPipelineFactory().GetPipeline();
            NioAcceptedSocketChannel newChannel = new NioAcceptedSocketChannel(GetFactory(), pipeline, this, sink, newSocket);
            if (log.IsInfoEnabled)
            {
                log.Info(" event=NioServerSocketChannel_Acpt channelId=" + newChannel.GetId() + " remoteEndpoint=" + newChannel.GetRemoteSocketAddress() + " localEndpoint=" + newChannel.GetLocalSocketAddress());
            }
        }

        internal int counterAcceptedConnections = 0;

        private void AcceptCallback(IAsyncResult result)
        {
            try
            {
                Socket newSocket = socket.EndAccept(result);

                NewSocketAccepted(newSocket);

                this.socket.BeginAccept(AcceptCallback, null);
            }
            catch (SocketException ex)
            {
                this.isAccepting = false;
                if (log.IsWarnEnabled)
                {
                    log.Warn(" event=AcceptCallbackSocketException", ex);
                }
            }
            catch (Exception ex)
            {
                this.isAccepting = false;
                if (log.IsWarnEnabled)
                {
                    log.Warn(" event=AcceptCallbackException", ex);
                }
            }
        }
    }
}
