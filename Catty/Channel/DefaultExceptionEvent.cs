using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catty.Core.Channel
{
    public class DefaultExceptionEvent : IExceptionEvent
    {
        private readonly IChannel channel;
        private readonly Exception cause;

        /**
         * Creates a new instance.
         */
        public DefaultExceptionEvent(IChannel channel, Exception cause)
        {
            if (channel == null)
            {
                throw new NullReferenceException("channel");
            }
            if (cause == null)
            {
                throw new NullReferenceException("cause");
            }
            this.channel = channel;
            this.cause = cause;
            //StackTraceSimplifier.simplify(cause);
        }

        public IChannel GetChannel()
        {
            return channel;
        }

        public IChannelFuture GetFuture()
        {
            return Channels.SucceededFuture(GetChannel());
        }

        public Exception GetCause()
        {
            return cause;
        }

        public override String ToString()
        {
            return GetChannel().ToString() + " EXCEPTION: " + cause;
        }
    }
}
