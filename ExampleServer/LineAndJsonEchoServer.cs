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
        private ServerBootstrap bootstrap;

        public LineAndJsonEchoServer()
        {
            // Configure the server.

            var factory = new NioServerSocketChannelFactory();
            bootstrap = new ServerBootstrap(factory);

            // Set up the pipeline factory.
            IChannelPipeline template = Channels.Pipeline(new LineBreakDecoder(), new StringEncoder(), new JsonEncoder(), new JsonEchoServerHandler());
            bootstrap.SetPipelineFactory(new ChannelPipelineFactoryByCloneExistPipeline(template));
        }

        public IChannel Bind(string hostPort)
        {
            // Bind and start to accept incoming connections.
            IPEndPoint addr = AddressRecordUtil.GetIPEndPointFromString(hostPort);
            return bootstrap.Bind(addr);
        }
    }

    public class LineAndJsonEchoServerWithNamedHandler
    {
        private ServerBootstrap bootstrap;
        public LineAndJsonEchoServerWithNamedHandler()
        {
            // Configure the server.

            var factory = new NioServerSocketChannelFactory();
            bootstrap = new ServerBootstrap(factory);

            // Set up the pipeline factory.
            IChannelPipelineFactory pipelineFactory = new MyFactory();
            bootstrap.SetPipelineFactory(pipelineFactory);
        }

        public class MyFactory : IChannelPipelineFactory
        {
            public IChannelPipeline GetPipeline()
            {
                DefaultChannelPipeline template = new DefaultChannelPipeline();
                template.AddLast("4", new StringEncoder());
                template.AddLast("2", new JsonEncoder());
                template.AddLast("1", new JsonEchoServerHandler());
                template.AddFirst("3", new LineBreakDecoder());
                return template;
            }
        }

        public IChannel Bind(string hostPort)
        {
            // Bind and start to accept incoming connections.
            IPEndPoint addr = AddressRecordUtil.GetIPEndPointFromString(hostPort);
            return bootstrap.Bind(addr);
        }
    }
}
