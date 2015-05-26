using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Channel
{
    public class FailedChannelFuture : CompleteChannelFuture
    {
        private readonly Exception cause;

        /**
         * Creates a new instance.
         *
         * @param channel the {@link Channel} associated with this future
         * @param cause   the cause of failure
         */
        public FailedChannelFuture(IChannel channel, Exception cause)
            : base(channel)
        {
            if (cause == null)
            {
                throw new NullReferenceException("cause");
            }
            this.cause = cause;
        }

        public override Exception GetCause()
        {
            return cause;
        }

        public override bool IsSuccess()
        {
            return false;
        }

    }
}
