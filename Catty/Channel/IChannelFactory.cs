using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Channel
{
    public interface IChannelFactory
    {
        /**
         * Creates and opens a new {@link Channel} and attaches the specified
         * {@link ChannelPipeline} to the new {@link Channel}.
         *
         * @param pipeline the {@link ChannelPipeline} which is going to be
         *                 attached to the new {@link Channel}
         *
         * @return the newly open channel
         *
         * @throws ChannelException if failed to create and open a new channel
         */
        IChannel NewChannel(IChannelPipeline pipeline);

        /**
         * Shudown the ChannelFactory and all the resource it created internal.
         */
        void Shutdown();
    }
}
