using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catty.Core.Channel
{
    public interface IChannelFutureProgressListener : IChannelFutureListener
    {
        /**
         * Invoked when the I/O operation associated with the {@link ChannelFuture}
         * has been progressed.
         *
         * @param future  the source {@link ChannelFuture} which called this
         *                callback
         */
        void OperationProgressed(IChannelFuture future, long amount, long current, long total);
    }
}
