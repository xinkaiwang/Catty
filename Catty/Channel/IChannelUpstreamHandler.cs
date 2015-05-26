using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catty.Core.Channel
{
    public interface IChannelUpstreamHandler : IChannelHandler
    {
        /**
         * Handles the specified upstream event.
         *
         * @param ctx  the context object for this handler
         * @param e    the upstream event to process or intercept
         */
        void HandleUpstream(IChannelHandlerContext ctx, IChannelEvent e);
    }
}
