using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catty.Core.Channel
{
    public interface IChannelHandlerContext
    {
        /**
         * Returns the {@link Channel} that the {@link ChannelPipeline} belongs to.
         * This method is a shortcut to <tt>getPipeline().getChannel()</tt>.
         */
        IChannel GetChannel();

        /**
         * Returns the {@link ChannelPipeline} that the {@link ChannelHandler}
         * belongs to.
         */
        IChannelPipeline GetPipeline();

        /**
         * Returns the name of the {@link ChannelHandler} in the
         * {@link ChannelPipeline}.
         */
        String GetName();

        /**
         * Returns the {@link ChannelHandler} that this context object is
         * serving.
         */
        IChannelHandler GetHandler();

        /**
         * Returns {@code true} if and only if the {@link ChannelHandler} is an
         * instance of {@link ChannelUpstreamHandler}.
         */
        bool CanHandleUpstream();

        /**
         * Returns {@code true} if and only if the {@link ChannelHandler} is an
         * instance of {@link ChannelDownstreamHandler}.
         */
        bool CanHandleDownstream();

        /**
         * Sends the specified {@link ChannelEvent} to the
         * {@link ChannelUpstreamHandler} which is placed in the closest upstream
         * from the handler associated with this context.  It is recommended to use
         * the shortcut methods in {@link Channels} rather than calling this method
         * directly.
         */
        void SendUpstream(IChannelEvent e);

        /**
         * Sends the specified {@link ChannelEvent} to the
         * {@link ChannelDownstreamHandler} which is placed in the closest
         * downstream from the handler associated with this context.  It is
         * recommended to use the shortcut methods in {@link Channels} rather than
         * calling this method directly.
         */
        void SendDownstream(IChannelEvent e);

        /**
         * Retrieves an object which is {@link #setAttachment(Object) attached} to
         * this context.
         *
         * @return {@code null} if no object was attached or
         *                      {@code null} was attached
         */
        Object GetAttachment();

        /**
         * Attaches an object to this context to store a stateful information
         * specific to the {@link ChannelHandler} which is associated with this
         * context.
         */
        void SetAttachment(Object attachment);
    }
}
