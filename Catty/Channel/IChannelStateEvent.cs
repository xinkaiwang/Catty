using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catty.Core.Channel
{
    public interface IChannelStateEvent : IChannelEvent
    {
        /**
         * Returns the changed property of the {@link Channel}.
         */
        ChannelState GetState();

        /**
         * Returns the value of the changed property of the {@link Channel}.
         * Please refer to {@link ChannelState} documentation to find out the
         * allowed values for each property.
         */
        Object GetValue();
    }
}
