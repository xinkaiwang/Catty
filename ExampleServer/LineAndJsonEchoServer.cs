using Catty.Core.Bootstrap;
using Catty.Core.Channel;
using Catty.Core.Handler.Codec;
using Catty.Core.Sockets.Nio;
using Catty.Core.Util;
using ExampleServer.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ExampleServer
{
    public class LineAndJsonEchoServer
    {
        private readonly string hostPort;

        public LineAndJsonEchoServer(string hostPort)
        {
            this.hostPort = hostPort;
        }

        public void Run()
        {
            // Configure the server.

            var factory = new NioServerSocketChannelFactory();
            ServerBootstrap bootstrap = new ServerBootstrap(factory);

            // Set up the pipeline factory.
            IChannelPipeline template = Channels.Pipeline(new LineBreakDecoder(), new StringEncoder(), new JsonEncoder(), new JsonEchoServerHandler());
            bootstrap.SetPipelineFactory(new Channels.ChannelPipelineFactoryByCloneExistPipeline(template));

            // Bind and start to accept incoming connections.
            IPEndPoint addr = AddressRecordUtil.GetIPEndPointFromString(this.hostPort);
            bootstrap.Bind(addr.Serialize());
        }

    }
}
