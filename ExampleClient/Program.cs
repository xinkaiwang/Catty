using Catty.Core.Bootstrap;
using Catty.Core.Channel;
using Catty.Core.Handler.Codec;
using Catty.Sockets.Nio;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleClient
{
    public class MyHandler : SimpleChannelUpstreamHandler
    {
        public override void MessageReceived(IChannelHandlerContext ctx, IMessageEvent e)
        {
            String msg = e.GetMessage() as string;
            if (msg != null)
            {
                Console.WriteLine(msg);
            }
        }

        public override void channelConnected(IChannelHandlerContext ctx, IChannelStateEvent e)
        {
            base.channelConnected(ctx, e);
            Channels.Write(ctx.GetChannel(), "abcd");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            BasicConfigurator.Configure();

            Func<IChannelHandler[]> handlersFactory = () => new IChannelHandler[] {new LineBreakDecoder(), new MyHandler()};

            // Configure the server.
            var factory = new NioClientSocketChannelFactory();
            var bootstrap = new ClientBootstrap(factory);

            // Set up the pipeline factory.
            bootstrap.SetPipelineFactory(handlersFactory);
            bootstrap.Connect(new IPEndPoint(IPAddress.Parse("10.0.1.5"), 8002));
            Thread.Sleep(100);
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
        }
    }
}
