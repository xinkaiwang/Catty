using Catty.Core.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExampleServer.Handler
{
    // take a string, return a object
    public class JsonEchoServerHandler : SimpleChannelUpstreamHandler
    {
        private long transferredBytes = 0;

        public long getTransferredBytes()
        {
            return Interlocked.Read(ref transferredBytes);
        }

        public override void MessageReceived(IChannelHandlerContext ctx, IMessageEvent e)
        {
            // Send back the received message to the remote peer.
            string str = e.GetMessage() as string;
            if (str != null)
            {
                Interlocked.Add(ref transferredBytes, str.Length);
                Channels.Write(e.GetChannel(), new { id = 10, line = str });
            }
        }

        public override void ExceptionCaught(IChannelHandlerContext ctx, IExceptionEvent e)
        {
            // Close the connection when an exception is raised.
            //logger.log(
            //        Level.WARNING,
            //        "Unexpected exception from downstream.",
            //        e.getCause());
            e.GetChannel().Close();
        }
    }
}
