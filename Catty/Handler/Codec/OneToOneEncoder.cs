using Catty.Core.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Handler.Codec
{
    public abstract class OneToOneEncoder : IChannelDownstreamHandler
    {
        protected OneToOneEncoder()
        {
        }

        public void HandleDownstream(IChannelHandlerContext ctx, IChannelEvent evt)
        {
            if (!(evt is IMessageEvent))
            {
                ctx.SendDownstream(evt);
                return;
            }

            IMessageEvent e = (IMessageEvent)evt;
            if (!DoEncode(ctx, e))
            {
                ctx.SendDownstream(e);
            }
        }

        protected bool DoEncode(IChannelHandlerContext ctx, IMessageEvent e)
        {
            Object originalMessage = e.GetMessage();
            Object encodedMessage = Encode(ctx, e.GetChannel(), originalMessage);
            if (originalMessage == encodedMessage)
            {
                return false;
            }
            if (encodedMessage != null)
            {
                Channels.Write(ctx, e.GetFuture(), encodedMessage, e.GetRemoteAddress());
            }
            return true;
        }

        /**
         * Transforms the specified message into another message and return the
         * transformed message.  Note that you can not return {@code null}, unlike
         * you can in {@link OneToOneDecoder#decode(ChannelHandlerContext, Channel, Object)};
         * you must return something, at least {@link ChannelBuffers#EMPTY_BUFFER}.
         */
        protected abstract Object Encode(IChannelHandlerContext ctx, IChannel channel, Object msg);
    }
}
