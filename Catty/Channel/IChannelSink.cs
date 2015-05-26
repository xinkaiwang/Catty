using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catty.Core.Channel
{

    /**
     * Receives and processes the terminal downstream {@link ChannelEvent}s.
     * <p>
     * A {@link ChannelSink} is an internal component which is supposed to be
     * implemented by a transport provider.  Most users will not see this type
     * in their code.
     *
     * @apiviz.uses org.jboss.netty.channel.ChannelPipeline - - sends events upstream
     */
    public interface IChannelSink
    {

        /**
         * Invoked by {@link ChannelPipeline} when a downstream {@link ChannelEvent}
         * has reached its terminal (the head of the pipeline).
         */
        void EventSunk(IChannelPipeline pipeline, IChannelEvent e);

        /**
         * Invoked by {@link ChannelPipeline} when an exception was raised while
         * one of its {@link ChannelHandler}s process a {@link ChannelEvent}.
         */
        void ExceptionCaught(IChannelPipeline pipeline, IChannelEvent e, ChannelPipelineException cause);

        /**
         * Execute the given {@link Runnable} later in the io-thread.
         * Some implementation may not support this and just execute it directly.
         */
        IChannelFuture Execute(IChannelPipeline pipeline, Action task);
    }
}
