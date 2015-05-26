using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catty.Core.Channel
{
    public class DownstreamChannelStateEvent : IChannelStateEvent
    {
        private readonly IChannel channel;
        private readonly IChannelFuture future;
        private readonly ChannelState state;
        private readonly Object value;

        /**
         * Creates a new instance.
         */
        public DownstreamChannelStateEvent(
                IChannel channel, IChannelFuture future,
                ChannelState state, Object value)
        {

            if (channel == null)
            {
                throw new NullReferenceException("channel");
            }
            if (future == null)
            {
                throw new NullReferenceException("future");
            }
            if (state == null)
            {
                throw new NullReferenceException("state");
            }
            this.channel = channel;
            this.future = future;
            this.state = state;
            this.value = value;
        }

        public IChannel GetChannel()
        {
            return channel;
        }

        public IChannelFuture GetFuture()
        {
            return future;
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
                        buf.Append(" CLOSE");
                    }
                    break;
                case ChannelState.BOUND:
                    if (GetValue() != null)
                    {
                        buf.Append(" BIND: ");
                        buf.Append(GetValue());
                    }
                    else
                    {
                        buf.Append(" UNBIND");
                    }
                    break;
                case ChannelState.CONNECTED:
                    if (GetValue() != null)
                    {
                        buf.Append(" CONNECT: ");
                        buf.Append(GetValue());
                    }
                    else
                    {
                        buf.Append(" DISCONNECT");
                    }
                    break;
                case ChannelState.INTEREST_OPS:
                    buf.Append(" CHANGE_INTEREST: ");
                    buf.Append(GetValue());
                    break;
                default:
                    buf.Append(' ');
                    buf.Append(GetState().ToString());
                    buf.Append(": ");
                    buf.Append(GetValue());
                    break;
            }
            return buf.ToString();
        }
    }
}
