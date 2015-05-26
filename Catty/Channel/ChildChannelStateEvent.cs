using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catty.Core.Channel
{
    public interface IChildChannelStateEvent : IChannelEvent
    {
        /**
         * Returns the <strong>parent</strong> {@link Channel} which is associated
         * with this event.  Please note that you should use {@link #getChildChannel()}
         * to get the {@link Channel} created or accepted by the parent {@link Channel}.
         */
        IChannel GetChannel();

        /**
         * Returns the child {@link Channel} whose state has been changed.
         */
        IChannel GetChildChannel();
    }
}
