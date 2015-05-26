using Catty.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Catty.Core.Channel
{
    public class UpstreamMessageEvent : IMessageEvent
    {

        private readonly IChannel channel;
        private readonly Object message;
        private readonly SocketAddress remoteAddress;

        /**
         * Creates a new instance.
         */
        public UpstreamMessageEvent(
                IChannel channel, Object message, SocketAddress remoteAddress)
        {

            if (channel == null)
            {
                throw new NullReferenceException("channel");
            }
            if (message == null)
            {
                throw new NullReferenceException("message");
            }
            this.channel = channel;
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
            return Channels.SucceededFuture(GetChannel());
        }

        public Object GetMessage()
        {
            return message;
        }

        public SocketAddress GetRemoteAddress()
        {
            return remoteAddress;
        }

        public override String ToString()
        {
            if (GetRemoteAddress() == GetChannel().GetRemoteAddress())
            {
                return GetChannel().ToString() + " RECEIVED: " +
                       StringUtil.StripControlCharacters(GetMessage());
            }
            else
            {
                return GetChannel().ToString() + " RECEIVED: " +
                       StringUtil.StripControlCharacters(GetMessage()) + " from " +
                       GetRemoteAddress();
            }
        }
    }
}
