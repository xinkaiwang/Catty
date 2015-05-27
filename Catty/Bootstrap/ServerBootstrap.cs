using Catty.Core.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Bootstrap
{
    public class ServerBootstrap : Bootstrap
    {
        private volatile IChannelHandler parentHandler;

        /**
         * Creates a new instance with no {@link ChannelFactory} set.
         * {@link #setFactory(ChannelFactory)} must be called before any I/O
         * operation is requested.
         */
        public ServerBootstrap()
        {
        }

        /**
         * Creates a new instance with the specified initial {@link ChannelFactory}.
         */
        public ServerBootstrap(IChannelFactory channelFactory)
            : base(channelFactory)
        {
        }

        /**
         * Returns an optional {@link ChannelHandler} which intercepts an event
         * of a newly bound server-side channel which accepts incoming connections.
         *
         * @return the parent channel handler.
         *         {@code null} if no parent channel handler is set.
         */
        public IChannelHandler GetParentHandler()
        {
            return parentHandler;
        }

        /**
         * Sets an optional {@link ChannelHandler} which intercepts an event of
         * a newly bound server-side channel which accepts incoming connections.
         *
         * @param parentHandler
         *        the parent channel handler.
         *        {@code null} to unset the current parent channel handler.
         */
        public void SetParentHandler(IChannelHandler parentHandler)
        {
            this.parentHandler = parentHandler;
        }

        /**
         * Creates a new channel which is bound to the specified local address. This operation will block until
         * the channel is bound.
         *
         * @return a new bound channel which accepts incoming connections
         *
         * @throws ChannelException
         *         if failed to create a new channel and
         *                      bind it to the local address
         */
        public IChannel Bind(SocketAddress localAddress)
        {
            IChannelFuture future = BindAsync(localAddress);

            // Wait for the future.
            future.AwaitUninterruptibly();
            if (!future.IsSuccess())
            {
                future.GetChannel().Close().AwaitUninterruptibly();
                throw new ChannelException("Failed to bind to: " + localAddress, future.GetCause());
            }

            return future.GetChannel();
        }

        /**
         * Bind a channel asynchronous to the specified local address.
         *
         * @return a new {@link ChannelFuture} which will be notified once the Channel is
         * bound and accepts incoming connections
         *
         */
        public IChannelFuture BindAsync(SocketAddress localAddress)
        {
            if (localAddress == null)
            {
                throw new NullReferenceException("localAddress");
            }
            Binder binder = new Binder(this, localAddress);
            IChannelHandler parentHandler = GetParentHandler();

            IChannelPipeline bossPipeline = Channels.Pipeline();
            bossPipeline.AddLast("binder", binder);
            if (parentHandler != null)
            {
                bossPipeline.AddLast("userHandler", parentHandler);
            }

            IChannel channel = GetFactory().NewChannel(bossPipeline);
            IChannelFuture bfuture = new DefaultChannelFuture(channel, false);
            binder.bindFuture.AddListener(future =>
            {
                if (future.IsSuccess())
                {
                    bfuture.SetSuccess();
                }
                else
                {
                    // Call close on bind failure
                    bfuture.GetChannel().Close();
                    bfuture.SetFailure(future.GetCause());
                }
            });
            return bfuture;
        }

        internal class Binder : SimpleChannelUpstreamHandler
        {
            private readonly ServerBootstrap parent;
            private readonly SocketAddress localAddress;
            private readonly Dictionary<String, Object> childOptions = new Dictionary<String, Object>();
            internal readonly DefaultChannelFuture bindFuture = new DefaultChannelFuture(null, false);
            internal Binder(ServerBootstrap parent, SocketAddress localAddress)
            {
                this.parent = parent;
                this.localAddress = localAddress;
            }

            public override void ChannelOpen(
                    IChannelHandlerContext ctx,
                    IChannelStateEvent evt)
            {

                try
                {
                    evt.GetChannel().GetConfig().SetPipelineFactory(parent.GetPipelineFactory());

                    // Split options into two categories: parent and child.
                    Dictionary<String, Object> allOptions = parent.GetOptions();
                    Dictionary<String, Object> parentOptions = new Dictionary<String, Object>();
                    foreach (var e in allOptions)
                    {
                        if (e.Key.StartsWith("child."))
                        {
                            childOptions.Add(e.Key.Substring(6), e.Value);
                        }
                        else if (!"pipelineFactory".Equals(e.Key))
                        {
                            parentOptions.Add(e.Key, e.Value);
                        }
                    }

                    // Apply parent options.
                    evt.GetChannel().GetConfig().SetOptions(parentOptions);
                }
                finally
                {
                    ctx.SendUpstream(evt);
                }

                evt.GetChannel().Bind(localAddress).AddListener((future) =>
                {
                    if (future.IsSuccess())
                    {
                        bindFuture.SetSuccess();
                    }
                    else
                    {
                        bindFuture.SetFailure(future.GetCause());
                    }
                });
            }

            public override void ChildChannelOpen(
                    IChannelHandlerContext ctx,
                    IChildChannelStateEvent e)
            {
                // Apply child options.
                try
                {
                    e.GetChildChannel().GetConfig().SetOptions(childOptions);
                }
                catch (Exception t)
                {
                    Channels.FireExceptionCaught(e.GetChildChannel(), t);
                }
                ctx.SendUpstream(e);
            }

            public override void ExceptionCaught(
                    IChannelHandlerContext ctx, IExceptionEvent e)
            {
                bindFuture.SetFailure(e.GetCause());
                ctx.SendUpstream(e);
            }
        }
    }
}
