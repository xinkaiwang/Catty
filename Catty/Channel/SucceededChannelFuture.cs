using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Channel
{
    public class SucceededChannelFuture : CompleteChannelFuture
    {
        /**
         * Creates a new instance.
         *
         * @param channel the {@link Channel} associated with this future
         */
        public SucceededChannelFuture(IChannel channel)
            : base(channel)
        {
        }

        public override Exception GetCause()
        {
            return null;
        }

        public override bool IsSuccess()
        {
            return true;
        }

        //public IChannelFuture sync() throws InterruptedException {
        //    return this;
        //}

        //public ChannelFuture syncUninterruptibly() {
        //    return this;
        //}
    }
}
