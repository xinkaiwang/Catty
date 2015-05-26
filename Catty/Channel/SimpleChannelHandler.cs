using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Channel
{
    public class SimpleChannelHandler : IChannelUpstreamHandler, IChannelDownstreamHandler
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
                        if (((bool)evt.GetValue()) == true)
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
                            ChannelConnected(ctx, evt);
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
        public virtual void MessageReceived(
                IChannelHandlerContext ctx, IMessageEvent e)
        {
            ctx.SendUpstream(e);
        }

        /**
         * Invoked when an exception was raised by an I/O thread or a
         * {@link ChannelHandler}.
         */
        public virtual void ExceptionCaught(
                IChannelHandlerContext ctx, IExceptionEvent e)
        {
            if (this == ctx.GetPipeline().GetLast())
            {
                //logger.warn(
                //        "EXCEPTION, please implement " + getClass().getName() +
                //        ".exceptionCaught() for proper handling.", e.getCause());
            }
            ctx.SendUpstream(e);
        }

        /**
         * Invoked when a {@link Channel} is open, but not bound nor connected.
         */
        public virtual void ChannelOpen(
                IChannelHandlerContext ctx, IChannelStateEvent e)
        {
            ctx.SendUpstream(e);
        }

        /**
         * Invoked when a {@link Channel} is open and bound to a local address,
         * but not connected.
         */
        public virtual void ChannelBound(
                IChannelHandlerContext ctx, IChannelStateEvent e)
        {
            ctx.SendUpstream(e);
        }

        /**
         * Invoked when a {@link Channel} is open, bound to a local address, and
         * connected to a remote address.
         */
        public virtual void ChannelConnected(
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

        /**
         * {@inheritDoc}  Down-casts the received downstream event into more
         * meaningful sub-type event and calls an appropriate handler method with
         * the down-casted event.
         */
        public void HandleDownstream(IChannelHandlerContext ctx, IChannelEvent e)
        {

            if (e is IMessageEvent)
            {
                WriteRequested(ctx, (IMessageEvent)e);
            }
            else if (e is IChannelStateEvent)
            {
                IChannelStateEvent evt = (IChannelStateEvent)e;
                switch (evt.GetState())
                {
                    case ChannelState.OPEN:
                        if (((bool)evt.GetValue()) == false)
                        {
                            CloseRequested(ctx, evt);
                        }
                        break;
                    case ChannelState.BOUND:
                        if (evt.GetValue() != null)
                        {
                            BindRequested(ctx, evt);
                        }
                        else
                        {
                            UnbindRequested(ctx, evt);
                        }
                        break;
                    case ChannelState.CONNECTED:
                        if (evt.GetValue() != null)
                        {
                            ConnectRequested(ctx, evt);
                        }
                        else
                        {
                            DisconnectRequested(ctx, evt);
                        }
                        break;
                    case ChannelState.INTEREST_OPS:
                        SetInterestOpsRequested(ctx, evt);
                        break;
                    default:
                        ctx.SendDownstream(e);
                        break;
                }
            }
            else
            {
                ctx.SendDownstream(e);
            }
        }

        /**
         * Invoked when {@link Channel#write(Object)} is called.
         */
        public virtual void WriteRequested(IChannelHandlerContext ctx, IMessageEvent e)
        {
            ctx.SendDownstream(e);
        }

        /**
         * Invoked when {@link Channel#bind(SocketAddress)} was called.
         */
        public virtual void BindRequested(IChannelHandlerContext ctx, IChannelStateEvent e)
        {
            ctx.SendDownstream(e);
        }

        /**
         * Invoked when {@link Channel#connect(SocketAddress)} was called.
         */
        public virtual void ConnectRequested(IChannelHandlerContext ctx, IChannelStateEvent e)
        {
            ctx.SendDownstream(e);
        }

        /**
         * Invoked when {@link Channel#setInterestOps(int)} was called.
         */
        public void SetInterestOpsRequested(IChannelHandlerContext ctx, IChannelStateEvent e)
        {
            ctx.SendDownstream(e);
        }

        /**
         * Invoked when {@link Channel#disconnect()} was called.
         */
        public virtual void DisconnectRequested(IChannelHandlerContext ctx, IChannelStateEvent e)
        {
            ctx.SendDownstream(e);
        }

        /**
         * Invoked when {@link Channel#unbind()} was called.
         */
        public virtual void UnbindRequested(IChannelHandlerContext ctx, IChannelStateEvent e)
        {
            ctx.SendDownstream(e);
        }

        /**
         * Invoked when {@link Channel#close()} was called.
         */
        public virtual void CloseRequested(IChannelHandlerContext ctx, IChannelStateEvent e)
        {
            ctx.SendDownstream(e);
        }
    }
}
