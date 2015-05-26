using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Channel
{
    public class DefaultChildChannelStateEvent : IChildChannelStateEvent
    {
        private readonly IChannel parentChannel;
        private readonly IChannel childChannel;

        /**
         * Creates a new instance.
         */
        public DefaultChildChannelStateEvent(IChannel parentChannel, IChannel childChannel)
        {
            if (parentChannel == null)
            {
                throw new NullReferenceException("parentChannel");
            }
            if (childChannel == null)
            {
                throw new NullReferenceException("childChannel");
            }
            this.parentChannel = parentChannel;
            this.childChannel = childChannel;
        }

        public IChannel GetChannel()
        {
            return parentChannel;
        }

        public IChannelFuture GetFuture()
        {
            return Channels.SucceededFuture(GetChannel());
        }

        public IChannel GetChildChannel()
        {
            return childChannel;
        }

        public override String ToString()
        {
            String channelString = GetChannel().ToString();
            StringBuilder buf = new StringBuilder(channelString.Length + 32);
            buf.Append(channelString);
            buf.Append(GetChildChannel().IsOpen() ? " CHILD_OPEN: " : " CHILD_CLOSED: ");
            buf.Append(GetChildChannel().GetId());
            return buf.ToString();
        }
    }
}
