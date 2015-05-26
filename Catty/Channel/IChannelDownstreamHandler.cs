using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Channel
{
    public interface IChannelDownstreamHandler : IChannelHandler
    {
        /**
         * Handles the specified downstream event.
         *
         * @param ctx  the context object for this handler
         * @param e    the downstream event to process or intercept
         */
        void HandleDownstream(IChannelHandlerContext ctx, IChannelEvent e);
    }
}
