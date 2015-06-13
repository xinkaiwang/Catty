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
    public class LineBreakDecoder : SimpleChannelUpstreamHandler, IChannelDownstreamHandler
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
                    if (str.ToCharArray()[index - 1] == '\r')
                    {
                        index = index - 1;
                    }
                    string item = str.Substring(0, index);
                    str = str.Substring(index + 1);
                    Channels.FireMessageReceived(ctx, item);
                }
                index = str.IndexOf('\n');
            } while (index > 0);
            cumulation = str;
        }

        public void HandleDownstream(IChannelHandlerContext ctx, IChannelEvent evt)
        {
            if (!(evt is IMessageEvent))
            {
                ctx.SendDownstream(evt);
                return;
            }

            IMessageEvent e = (IMessageEvent)evt;
            Object originalMessage = e.GetMessage();
            if (originalMessage != null)
            {
                if (originalMessage is String)
                {
                    IChannelBuffer buf = ChannelBuffers.DynamicBuffer(100);
                    byte[] bytes = DataTypeString.BytesFromString((string)originalMessage);
                    buf.WriteBytes(bytes, 0, bytes.Length);
                    buf.WriteByte((byte)'\n');
                    Channels.Write(ctx.GetChannel(), buf);
                    return;
                }
            }
            ctx.SendDownstream(evt);
        }
    }
}
