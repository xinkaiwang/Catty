using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catty.Core.Channel
{
    public class WriteCompletionEvent : IChannelEvent
    {
        private readonly IChannel channel;
        private readonly long writtenAmount;

        /**
         * Creates a new instance.
         */
        public WriteCompletionEvent(IChannel channel, long writtenAmount)
        {
            if (channel == null)
            {
                throw new NullReferenceException("channel");
            }
            if (writtenAmount <= 0)
            {
                throw new ArgumentOutOfRangeException(
                        "writtenAmount must be a positive integer: " + writtenAmount);
            }

            this.channel = channel;
            this.writtenAmount = writtenAmount;
        }

        public IChannel GetChannel()
        {
            return channel;
        }

        public IChannelFuture GetFuture()
        {
            return Channels.SucceededFuture(GetChannel());
        }

        public long GetWrittenAmount()
        {
            return writtenAmount;
        }

        public override String ToString()
        {
            String channelString = GetChannel().ToString();
            StringBuilder buf = new StringBuilder(channelString.Length + 32);
            buf.Append(channelString);
            buf.Append(" WRITTEN_AMOUNT: ");
            buf.Append(GetWrittenAmount());
            return buf.ToString();
        }
    }
}
