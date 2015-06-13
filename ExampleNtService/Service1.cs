using Catty.Core.Bootstrap;
using Catty.Core.Channel;
using Catty.Core.Handler.Codec;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ExampleNtService
{
    public partial class Service1 : SimpleNtService
    {
        public Service1()
        {
            InitializeComponent();
            this.SetPort(new IPEndPoint(IPAddress.Any, 8003));
            this.SetHandlerFactory(() => new IChannelHandler[] { new LineBreakDecoder(), new StringEncoder(), new MyHandler() });
        }
    }

    public class MyHandler : SimpleChannelUpstreamHandler
    {
        public override void MessageReceived(
                IChannelHandlerContext ctx, IMessageEvent e)
        {
            string msg = e.GetMessage() as string;
            if (msg != null)
            {
                msg = msg.ToUpper();
                Console.WriteLine(msg);
                Channels.Write(ctx.GetChannel(), msg);
            }
        }
    }
}
