using Catty.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Channel
{
    public class DownstreamMessageEvent : IMessageEvent
    {
        private readonly IChannel channel;
        private readonly IChannelFuture future;
        private readonly Object message;
        private readonly EndPoint remoteAddress;

        /**
         * Creates a new instance.
         */
        public DownstreamMessageEvent(
                IChannel channel, IChannelFuture future,
                Object message, EndPoint remoteAddress)
        {

            if (channel == null)
            {
                throw new NullReferenceException("channel");
            }
            if (future == null)
            {
                throw new NullReferenceException("future");
            }
            if (message == null)
            {
                throw new NullReferenceException("message");
            }
            this.channel = channel;
            this.future = future;
            this.message = message;
            if (remoteAddress != null)
            {
                this.remoteAddress = remoteAddress;
            }
            else
            {
                this.remoteAddress = channel.GetRemoteAddress();
            }
        }

        public IChannel GetChannel()
        {
            return channel;
        }

        public IChannelFuture GetFuture()
        {
            return future;
        }

        public Object GetMessage()
        {
            return message;
        }

        public EndPoint GetRemoteAddress()
        {
            return remoteAddress;
        }

        public override String ToString()
        {
            if (GetRemoteAddress() == GetChannel().GetRemoteAddress())
            {
                return GetChannel().ToString() + " WRITE: " +
                       StringUtil.StripControlCharacters(GetMessage());
            }
            else
            {
                return GetChannel().ToString() + " WRITE: " +
                       StringUtil.StripControlCharacters(GetMessage()) + " to " +
                       GetRemoteAddress();
            }
        }
    }
}
