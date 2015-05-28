using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Channel
{
    public class Channels
    {
        public static IChannelPipeline Pipeline(params IChannelHandler[] handlers)
        {
            if (handlers == null)
            {
                throw new NullReferenceException("handlers");
            }

            IChannelPipeline newPipeline = new DefaultChannelPipeline();
            for (int i = 0; i < handlers.Length; i++)
            {
                IChannelHandler h = handlers[i];
                if (h == null)
                {
                    break;
                }
                newPipeline.AddLast(i.ToString(), h);
            }
            return newPipeline;
        }

        public static IChannelPipelineFactory PipelineFactory(IChannelPipeline pipeline)
        {
            return new ChannelPipelineFactoryByCloneExistPipeline(pipeline);
        }

        // future factory methods

        /**
         * Creates a new non-cancellable {@link ChannelFuture} for the specified
         * {@link Channel}.
         */
        public static IChannelFuture Future(IChannel channel)
        {
            return Future(channel, false);
        }

        /**
         * Creates a new {@link ChannelFuture} for the specified {@link Channel}.
         *
         * @param cancellable {@code true} if and only if the returned future
         *                    can be canceled by {@link ChannelFuture#cancel()}
         */
        public static IChannelFuture Future(IChannel channel, bool cancellable)
        {
            return new DefaultChannelFuture(channel, cancellable);
        }

        internal static IChannelFuture FailedFuture(IChannel channel, Exception cause)
        {
            return new FailedChannelFuture(channel, cause);
        }

        internal static IChannelFuture SucceededFuture(IChannel channel)
        {
            if (channel is AbstractChannel)
            {
                return ((AbstractChannel)channel).GetSucceededFuture();
            }
            else
            {
                return new SucceededChannelFuture(channel);
            }
        }

        /**
         * Sends a {@code "messageReceived"} event to the first
         * {@link ChannelUpstreamHandler} in the {@link ChannelPipeline} of
         * the specified {@link Channel}.
         *
         * @param message  the received message
         */
        public static void FireMessageReceived(IChannel channel, Object message)
        {
            FireMessageReceived(channel, message, null);
        }

        /**
         * Sends a {@code "messageReceived"} event to the first
         * {@link ChannelUpstreamHandler} in the {@link ChannelPipeline} of
         * the specified {@link Channel} belongs.
         *
         * @param message        the received message
         * @param remoteAddress  the remote address where the received message
         *                       came from
         */
        public static void FireMessageReceived(IChannel channel, Object message, SocketAddress remoteAddress)
        {
            channel.GetPipeline().SendUpstream(
                    new UpstreamMessageEvent(channel, message, remoteAddress));
        }

        /**
         * Sends a {@code "messageReceived"} event to the
         * {@link ChannelUpstreamHandler} which is placed in the closest upstream
         * from the handler associated with the specified
         * {@link ChannelHandlerContext}.
         *
         * @param message  the received message
         */
        public static void FireMessageReceived(IChannelHandlerContext ctx, Object message)
        {
            ctx.SendUpstream(new UpstreamMessageEvent(ctx.GetChannel(), message, null));
        }

        /**
         * Sends a {@code "messageReceived"} event to the
         * {@link ChannelUpstreamHandler} which is placed in the closest upstream
         * from the handler associated with the specified
         * {@link ChannelHandlerContext}.
         *
         * @param message        the received message
         * @param remoteAddress  the remote address where the received message
         *                       came from
         */
        public static void FireMessageReceived(
                IChannelHandlerContext ctx, Object message, SocketAddress remoteAddress)
        {
            ctx.SendUpstream(new UpstreamMessageEvent(
                    ctx.GetChannel(), message, remoteAddress));
        }

        /**
         * Sends a {@code "exceptionCaught"} event to the first
         * {@link ChannelUpstreamHandler} in the {@link ChannelPipeline} of
         * the specified {@link Channel} once the io-thread runs again.
         */
        public static IChannelFuture FireExceptionCaughtLater(IChannel channel, Exception cause)
        {
            return channel.GetPipeline().Execute(() => FireExceptionCaught(channel, cause));
        }

        /**
         * Sends a {@code "exceptionCaught"} event to the
         * {@link ChannelUpstreamHandler} which is placed in the closest upstream
         * from the handler associated with the specified
         * {@link ChannelHandlerContext} once the io-thread runs again.
         */
        public static IChannelFuture FireExceptionCaughtLater(IChannelHandlerContext ctx, Exception cause)
        {
            return ctx.GetPipeline().Execute(() => FireExceptionCaught(ctx, cause));
        }

        /**
         * Sends a {@code "exceptionCaught"} event to the first
         * {@link ChannelUpstreamHandler} in the {@link ChannelPipeline} of
         * the specified {@link Channel}.
         */
        public static void FireExceptionCaught(IChannel channel, Exception cause)
        {
            channel.GetPipeline().SendUpstream(
                    new DefaultExceptionEvent(channel, cause));
        }

        /**
         * Sends a {@code "exceptionCaught"} event to the
         * {@link ChannelUpstreamHandler} which is placed in the closest upstream
         * from the handler associated with the specified
         * {@link ChannelHandlerContext}.
         */
        public static void FireExceptionCaught(IChannelHandlerContext ctx, Exception cause)
        {
            ctx.SendUpstream(new DefaultExceptionEvent(ctx.GetChannel(), cause));
        }

        private static void FireChildChannelStateChanged(IChannel channel, IChannel childChannel)
        {
            channel.GetPipeline().SendUpstream(new DefaultChildChannelStateEvent(channel, childChannel));
        }

        internal static void FireChannelOpen(IChannel channel)
        {
            // Notify the parent handler.
            if (channel.GetParent() != null)
            {
                FireChildChannelStateChanged(channel.GetParent(), channel);
            }

            channel.GetPipeline().SendUpstream(
                    new UpstreamChannelStateEvent(
                            channel, ChannelState.OPEN, true));
        }

        /**
         * Sends a {@code "channelBound"} event to the first
         * {@link ChannelUpstreamHandler} in the {@link ChannelPipeline} of
         * the specified {@link Channel}.
         *
         * @param localAddress
         *        the local address where the specified channel is bound
         */
        public static void FireChannelBound(IChannel channel, SocketAddress localAddress)
        {
            channel.GetPipeline().SendUpstream(
                    new UpstreamChannelStateEvent(
                            channel, ChannelState.BOUND, localAddress));
        }

        /**
         * Sends a {@code "channelBound"} event to the
         * {@link ChannelUpstreamHandler} which is placed in the closest upstream
         * from the handler associated with the specified
         * {@link ChannelHandlerContext}.
         *
         * @param localAddress
         *        the local address where the specified channel is bound
         */
        public static void FireChannelBound(IChannelHandlerContext ctx, SocketAddress localAddress)
        {
            ctx.SendUpstream(new UpstreamChannelStateEvent(
                    ctx.GetChannel(), ChannelState.BOUND, localAddress));
        }

        /**
         * Sends a {@code "bind"} request to the last
         * {@link ChannelDownstreamHandler} in the {@link ChannelPipeline} of
         * the specified {@link Channel}.
         *
         * @param channel  the channel to bind
         * @param localAddress  the local address to bind to
         *
         * @return the {@link ChannelFuture} which will be notified when the
         *         bind operation is done
         */
        public static IChannelFuture Bind(IChannel channel, SocketAddress localAddress)
        {
            if (localAddress == null)
            {
                throw new NullReferenceException("localAddress");
            }
            IChannelFuture future = Future(channel);
            channel.GetPipeline().SendDownstream(new DownstreamChannelStateEvent(
                    channel, future, ChannelState.BOUND, localAddress));
            return future;
        }

        /**
         * Sends a {@code "bind"} request to the
         * {@link ChannelDownstreamHandler} which is placed in the closest
         * downstream from the handler associated with the specified
         * {@link ChannelHandlerContext}.
         *
         * @param ctx     the context
         * @param future  the future which will be notified when the bind
         *                operation is done
         * @param localAddress the local address to bind to
         */
        public static void Bind(IChannelHandlerContext ctx, IChannelFuture future, SocketAddress localAddress)
        {
            if (localAddress == null)
            {
                throw new NullReferenceException("localAddress");
            }
            ctx.SendDownstream(new DownstreamChannelStateEvent(
                    ctx.GetChannel(), future, ChannelState.BOUND, localAddress));
        }

        /**
         * Sends a {@code "unbind"} request to the
         * {@link ChannelDownstreamHandler} which is placed in the closest
         * downstream from the handler associated with the specified
         * {@link ChannelHandlerContext}.
         *
         * @param ctx     the context
         * @param future  the future which will be notified when the unbind
         *                operation is done
         */
        public static void Unbind(IChannelHandlerContext ctx, IChannelFuture future)
        {
            ctx.SendDownstream(new DownstreamChannelStateEvent(
                    ctx.GetChannel(), future, ChannelState.BOUND, null));
        }

        /**
         * Sends a {@code "unbind"} request to the last
         * {@link ChannelDownstreamHandler} in the {@link ChannelPipeline} of
         * the specified {@link Channel}.
         *
         * @param channel  the channel to unbind
         *
         * @return the {@link ChannelFuture} which will be notified when the
         *         unbind operation is done
         */
        public static IChannelFuture Unbind(IChannel channel)
        {
            IChannelFuture future = Future(channel);
            channel.GetPipeline().SendDownstream(new DownstreamChannelStateEvent(
                    channel, future, ChannelState.BOUND, null));
            return future;
        }

        /**
         * Sends a {@code "write"} request to the last
         * {@link ChannelDownstreamHandler} in the {@link ChannelPipeline} of
         * the specified {@link Channel}.
         *
         * @param channel  the channel to write a message
         * @param message  the message to write to the channel
         *
         * @return the {@link ChannelFuture} which will be notified when the
         *         write operation is done
         */
        public static IChannelFuture Write(IChannel channel, Object message)
        {
            return Write(channel, message, null);
        }

        /**
         * Sends a {@code "write"} request to the
         * {@link ChannelDownstreamHandler} which is placed in the closest
         * downstream from the handler associated with the specified
         * {@link ChannelHandlerContext}.
         *
         * @param ctx     the context
         * @param future  the future which will be notified when the write
         *                operation is done
         */
        public static void Write(
                IChannelHandlerContext ctx, IChannelFuture future, Object message)
        {
            Write(ctx, future, message, null);
        }

        /**
         * Sends a {@code "write"} request to the last
         * {@link ChannelDownstreamHandler} in the {@link ChannelPipeline} of
         * the specified {@link Channel}.
         *
         * @param channel  the channel to write a message
         * @param message  the message to write to the channel
         * @param remoteAddress  the destination of the message.
         *                       {@code null} to use the default remote address
         *
         * @return the {@link ChannelFuture} which will be notified when the
         *         write operation is done
         */
        public static IChannelFuture Write(IChannel channel, Object message, SocketAddress remoteAddress)
        {
            IChannelFuture future = Future(channel);
            channel.GetPipeline().SendDownstream(
                    new DownstreamMessageEvent(channel, future, message, remoteAddress));
            return future;
        }

        /**
         * Sends a {@code "write"} request to the
         * {@link ChannelDownstreamHandler} which is placed in the closest
         * downstream from the handler associated with the specified
         * {@link ChannelHandlerContext}.
         *
         * @param ctx     the context
         * @param future  the future which will be notified when the write
         *                operation is done
         * @param message the message to write to the channel
         * @param remoteAddress  the destination of the message.
         *                       {@code null} to use the default remote address.
         */
        public static void Write(IChannelHandlerContext ctx, IChannelFuture future, Object message, SocketAddress remoteAddress)
        {
            ctx.SendDownstream(
                    new DownstreamMessageEvent(ctx.GetChannel(), future, message, remoteAddress));
        }

        /**
         * Sends a {@code "close"} request to the last
         * {@link ChannelDownstreamHandler} in the {@link ChannelPipeline} of
         * the specified {@link Channel}.
         *
         * @param channel  the channel to close
         *
         * @return the {@link ChannelFuture} which will be notified on closure
         */
        public static IChannelFuture Close(IChannel channel)
        {
            IChannelFuture future = channel.GetCloseFuture();
            channel.GetPipeline().SendDownstream(new DownstreamChannelStateEvent(
                    channel, future, ChannelState.OPEN, false));
            return future;
        }

        /**
         * Sends a {@code "close"} request to the
         * {@link ChannelDownstreamHandler} which is placed in the closest
         * downstream from the handler associated with the specified
         * {@link ChannelHandlerContext}.
         *
         * @param ctx     the context
         * @param future  the future which will be notified on closure
         */
        public static void Close(IChannelHandlerContext ctx, IChannelFuture future)
        {
            ctx.SendDownstream(new DownstreamChannelStateEvent(
                    ctx.GetChannel(), future, ChannelState.OPEN, false));
        }
    }

    /**
     * Creates a new {@link ChannelPipelineFactory} which creates a new
     * {@link ChannelPipeline} which contains the same entries with the
     * specified {@code pipeline}.  Please note that only the names and the
     * references of the {@link ChannelHandler}s will be copied; a new
     * {@link ChannelHandler} instance will never be created.
     */
    public class ChannelPipelineFactoryByCloneExistPipeline : IChannelPipelineFactory
    {
        IChannelPipeline pipeline;
        public ChannelPipelineFactoryByCloneExistPipeline(IChannelPipeline pipeline)
        {
            this.pipeline = pipeline;
        }
        public IChannelPipeline GetPipeline()
        {
            IChannelPipeline newPipeline = new DefaultChannelPipeline();
            foreach (var e in pipeline.ToMap())
            {
                newPipeline.AddLast(e.Key, e.Value);
            }
            return newPipeline;
        }
    }
}
