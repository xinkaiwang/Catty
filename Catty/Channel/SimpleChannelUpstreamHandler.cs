using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Channel
{
    public abstract class SimpleChannelUpstreamHandler : IChannelUpstreamHandler
    {
        /**
         * {@inheritDoc}  Down-casts the received upstream event into more
         * meaningful sub-type event and calls an appropriate handler method with
         * the down-casted event.
         */
        public void HandleUpstream(
                IChannelHandlerContext ctx, IChannelEvent e)
        {

            if (e is IMessageEvent)
            {
                MessageReceived(ctx, (IMessageEvent)e);
            }
            else if (e is WriteCompletionEvent)
            {
                WriteCompletionEvent evt = (WriteCompletionEvent)e;
                WriteComplete(ctx, evt);
            }
            else if (e is IChildChannelStateEvent)
            {
                IChildChannelStateEvent evt = (IChildChannelStateEvent)e;
                if (evt.GetChildChannel().IsOpen())
                {
                    ChildChannelOpen(ctx, evt);
                }
                else
                {
                    ChildChannelClosed(ctx, evt);
                }
            }
            else if (e is IChannelStateEvent)
            {
                IChannelStateEvent evt = (IChannelStateEvent)e;
                switch (evt.GetState())
                {
                    case ChannelState.OPEN:
                        if ((bool)(evt.GetValue()) == true)
                        {
                            ChannelOpen(ctx, evt);
                        }
                        else
                        {
                            ChannelClosed(ctx, evt);
                        }
                        break;
                    case ChannelState.BOUND:
                        if (evt.GetValue() != null)
                        {
                            ChannelBound(ctx, evt);
                        }
                        else
                        {
                            ChannelUnbound(ctx, evt);
                        }
                        break;
                    case ChannelState.CONNECTED:
                        if (evt.GetValue() != null)
                        {
                            channelConnected(ctx, evt);
                        }
                        else
                        {
                            ChannelDisconnected(ctx, evt);
                        }
                        break;
                    case ChannelState.INTEREST_OPS:
                        ChannelInterestChanged(ctx, evt);
                        break;
                    default:
                        ctx.SendUpstream(e);
                        break;
                }
            }
            else if (e is IExceptionEvent)
            {
                ExceptionCaught(ctx, (IExceptionEvent)e);
            }
            else
            {
                ctx.SendUpstream(e);
            }
        }

        /**
         * Invoked when a message object (e.g: {@link ChannelBuffer}) was received
         * from a remote peer.
         */
        public abstract void MessageReceived(IChannelHandlerContext ctx, IMessageEvent e);

        /**
         * Invoked when an exception was raised by an I/O thread or a
         * {@link ChannelHandler}.
         */
        public virtual void ExceptionCaught(
                IChannelHandlerContext ctx, IExceptionEvent e)
        {
            IChannelPipeline pipeline = ctx.GetPipeline();

            IChannelHandler last = pipeline.GetLast();
            if (!(last is IChannelUpstreamHandler) && ctx is DefaultChannelPipeline)
            {
                // The names comes in the order of which they are insert when using DefaultChannelPipeline
                List<String> names = ctx.GetPipeline().GetNames();
                for (int i = names.Count - 1; i >= 0; i--)
                {
                    IChannelHandler handler = ctx.GetPipeline().Get(names[i]);
                    if (handler is IChannelUpstreamHandler)
                    {
                        // find the last handler
                        last = handler;
                        break;
                    }
                }
            }
            if (this == last)
            {
                //logger.warn(
                //        "EXCEPTION, please implement " + getClass().getName() +
                //        ".exceptionCaught() for proper handling.", e.getCause());
            }
            ctx.SendUpstream(e);
        }

        /**
         * Invoked when a {@link Channel} is open, but not bound nor connected.
         * <br/>
         *
         * <strong>Be aware that this event is fired from within the Boss-Thread so you should not
         * execute any heavy operation in there as it will block the dispatching to other workers!</strong>
         */
        public virtual void ChannelOpen(
                IChannelHandlerContext ctx, IChannelStateEvent e)
        {
            ctx.SendUpstream(e);
        }

        /**
         * Invoked when a {@link Channel} is open and bound to a local address,
         * but not connected.
         * <br/>
         *
         * <strong>Be aware that this event is fired from within the Boss-Thread so you should not
         * execute any heavy operation in there as it will block the dispatching to other workers!</strong>
         */
        public virtual void ChannelBound(
                IChannelHandlerContext ctx, IChannelStateEvent e)
        {
            ctx.SendUpstream(e);
        }

        /**
         * Invoked when a {@link Channel} is open, bound to a local address, and
         * connected to a remote address.
         * <br/>
         *
         * <strong>Be aware that this event is fired from within the Boss-Thread so you should not
         * execute any heavy operation in there as it will block the dispatching to other workers!</strong>
         */
        public virtual void channelConnected(
                IChannelHandlerContext ctx, IChannelStateEvent e)
        {
            ctx.SendUpstream(e);
        }

        /**
         * Invoked when a {@link Channel}'s {@link Channel#getInterestOps() interestOps}
         * was changed.
         */
        public virtual void ChannelInterestChanged(
                IChannelHandlerContext ctx, IChannelStateEvent e)
        {
            ctx.SendUpstream(e);
        }

        /**
         * Invoked when a {@link Channel} was disconnected from its remote peer.
         */
        public virtual void ChannelDisconnected(
                IChannelHandlerContext ctx, IChannelStateEvent e)
        {
            ctx.SendUpstream(e);
        }

        /**
         * Invoked when a {@link Channel} was unbound from the current local address.
         */
        public virtual void ChannelUnbound(
                IChannelHandlerContext ctx, IChannelStateEvent e)
        {
            ctx.SendUpstream(e);
        }

        /**
         * Invoked when a {@link Channel} was closed and all its related resources
         * were released.
         */
        public virtual void ChannelClosed(
                IChannelHandlerContext ctx, IChannelStateEvent e)
        {
            ctx.SendUpstream(e);
        }

        /**
         * Invoked when something was written into a {@link Channel}.
         */
        public virtual void WriteComplete(
                IChannelHandlerContext ctx, WriteCompletionEvent e)
        {
            ctx.SendUpstream(e);
        }

        /**
         * Invoked when a child {@link Channel} was open.
         * (e.g. a server channel accepted a connection)
         */
        public virtual void ChildChannelOpen(
                IChannelHandlerContext ctx, IChildChannelStateEvent e)
        {
            ctx.SendUpstream(e);
        }

        /**
         * Invoked when a child {@link Channel} was closed.
         * (e.g. the accepted connection was closed)
         */
        public virtual void ChildChannelClosed(
                IChannelHandlerContext ctx, IChildChannelStateEvent e)
        {
            ctx.SendUpstream(e);
        }
    }
}
