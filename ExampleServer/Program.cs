using Catty;
using Catty.Bootstrap;
using Catty.Core.Buffer;
using Catty.Core.Channel;
using Catty.Core.Handler.Codec;
using ExampleServer.Handler;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ExampleServer
{

    public class MyHandler : SimpleChannelUpstreamHandler
    {
        public override void MessageReceived(
                IChannelHandlerContext ctx, IMessageEvent e)
        {
            string msg = e.GetMessage() as string;
            if (msg != null)
            {
                Console.WriteLine(msg);
                Channels.Write(ctx.GetChannel(), msg);
                ctx.GetChannel().Write(msg);
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            BasicConfigurator.Configure();

            Func<IChannelHandler[]> handlersFactory = () => new IChannelHandler[] {new LineBreakDecoder(), new MyHandler()};
            var server = new SimpleTcpService().SetHandlers(handlersFactory);
            server.Bind(new IPEndPoint(IPAddress.Any, 8002));
            Console.WriteLine("server started ...");
            new CtrlCListener().WaitForEvent();
            Console.WriteLine("server exiting ....");
        }

    }
}
