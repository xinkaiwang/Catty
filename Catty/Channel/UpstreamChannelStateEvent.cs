using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catty.Core.Channel
{
    public class UpstreamChannelStateEvent : IChannelStateEvent
    {
        private readonly IChannel channel;
        private readonly ChannelState state;
        private readonly Object value;

        /**
         * Creates a new instance.
         */
        public UpstreamChannelStateEvent(
                IChannel channel, ChannelState state, Object value)
        {

            if (channel == null)
            {
                throw new NullReferenceException("channel");
            }
            if (state == null)
            {
                throw new NullReferenceException("state");
            }

            this.channel = channel;
            this.state = state;
            this.value = value;
        }

        public IChannel GetChannel()
        {
            return channel;
        }

        public IChannelFuture GetFuture()
        {
            return Channels.SucceededFuture(GetChannel());
        }

        public ChannelState GetState()
        {
            return state;
        }

        public Object GetValue()
        {
            return value;
        }

        public override String ToString()
        {
            String channelString = GetChannel().ToString();
            StringBuilder buf = new StringBuilder(channelString.Length + 64);
            buf.Append(channelString);
            switch (GetState())
            {
                case ChannelState.OPEN:
                    if (((bool)GetValue()) == true)
                    {
                        buf.Append(" OPEN");
                    }
                    else
                    {
                        buf.Append(" CLOSED");
                    }
                    break;
                case ChannelState.BOUND:
                    if (GetValue() != null)
                    {
                        buf.Append(" BOUND: ");
                        buf.Append(GetValue());
                    }
                    else
                    {
                        buf.Append(" UNBOUND");
                    }
                    break;
                case ChannelState.CONNECTED:
                    if (GetValue() != null)
                    {
                        buf.Append(" CONNECTED: ");
                        buf.Append(GetValue());
                    }
                    else
                    {
                        buf.Append(" DISCONNECTED");
                    }
                    break;
                case ChannelState.INTEREST_OPS:
                    buf.Append(" INTEREST_CHANGED");
                    break;
                default:
                    buf.Append(GetState().ToString());
                    buf.Append(": ");
                    buf.Append(GetValue());
                    break;
            }
            return buf.ToString();
        }
    }
}
