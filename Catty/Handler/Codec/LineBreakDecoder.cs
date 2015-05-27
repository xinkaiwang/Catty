using Catty.Core.Buffer;
using Catty.Core.Channel;
using Catty.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Handler.Codec
{
    // 
    public class LineBreakDecoder : SimpleChannelUpstreamHandler
    {
        protected string cumulation;

        public override void MessageReceived(
                IChannelHandlerContext ctx, IMessageEvent e)
        {

            Object m = e.GetMessage();
            if (!(m is IChannelBuffer))
            {
                ctx.SendUpstream(e);
                return;
            }

            IChannelBuffer input = (IChannelBuffer)m;
            if (!input.Readable())
            {
                return;
            }

            var bytes = input.GetByteArray();
            string str = DataTypeString.StringFromBytes(bytes);
            if (cumulation != null)
            {
                str = cumulation + str;
            }
            int index = str.IndexOf('\n');
            do
            {
                if (index > 0)
                {
                    string item = str.Substring(0, index - 1);
                    str = str.Substring(index + 1);
                    Channels.FireMessageReceived(ctx, item);
                }
                index = str.IndexOf('\n');
            } while (index > 0);
            cumulation = str;
        }
    }
}
