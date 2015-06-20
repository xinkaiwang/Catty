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
            if (!(m is IByteBuf))
            {
                ctx.SendUpstream(e);
                return;
            }

            IByteBuf input = (IByteBuf)m;

            var bytes = input.GetByteArray();
            string str = DataTypeString.StringFromBytes(bytes);
            if (cumulation != null)
            {
                str = cumulation + str;
            }
            int index = str.IndexOf('\n');
            do
            {
                if (index >= 0)
                {
                    int trimEnd = index;
                    if (str.ToCharArray()[index - 1] == '\r')
                    {
                        trimEnd -= 1;
                    }
                    string item = str.Substring(0, trimEnd);
                    str = str.Substring(index + 1);
                    Channels.FireMessageReceived(ctx, item);
                }
                index = str.IndexOf('\n');
            } while (index >= 0);
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
                    IByteBuf buf = DynamicByteBuf.GetInstance();
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
