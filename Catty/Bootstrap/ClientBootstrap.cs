using Catty.Core.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Bootstrap
{
    public class ClientBootstrap : Bootstrap
    {
        /**
         * Creates a new instance with no {@link ChannelFactory} set.
         * {@link #setFactory(ChannelFactory)} must be called before any I/O
         * operation is requested.
         */
        public ClientBootstrap()
        {
        }

        /**
         * Creates a new instance with the specified initial {@link ChannelFactory}.
         */
        public ClientBootstrap(IChannelFactory channelFactory)
            : base(channelFactory)
        {
        }

        /**
         * Attempts a new connection with the current {@code "remoteAddress"} and
         * {@code "localAddress"} option.  If the {@code "localAddress"} option is
         * not set, the local address of a new channel is determined automatically.
         * This method is similar to the following code:
         *
         * <pre>
         * {@link ClientBootstrap} b = ...;
         * b.connect(b.getOption("remoteAddress"), b.getOption("localAddress"));
         * </pre>
         *
         * @return a future object which notifies when this connection attempt
         *         succeeds or fails
         *
         * @throws IllegalStateException
         *         if {@code "remoteAddress"} option was not set
         * @throws ClassCastException
         *         if {@code "remoteAddress"} or {@code "localAddress"} option's
         *            value is neither a {@link SocketAddress} nor {@code null}
         * @throws ChannelPipelineException
         *         if this bootstrap's {@link #setPipelineFactory(ChannelPipelineFactory) pipelineFactory}
         *            failed to create a new {@link ChannelPipeline}
         */
        public IChannelFuture Connect()
        {
            EndPoint remoteAddress = (EndPoint)GetOption("remoteAddress");
            if (remoteAddress == null)
            {
                throw new ArgumentNullException("remoteAddress option is not set.");
            }
            return Connect(remoteAddress);
        }

        /**
         * Attempts a new connection with the specified {@code remoteAddress} and
         * the current {@code "localAddress"} option. If the {@code "localAddress"}
         * option is not set, the local address of a new channel is determined
         * automatically.  This method is identical with the following code:
         *
         * <pre>
         * {@link ClientBootstrap} b = ...;
         * b.connect(remoteAddress, b.getOption("localAddress"));
         * </pre>
         *
         * @return a future object which notifies when this connection attempt
         *         succeeds or fails
         *
         * @throws ClassCastException
         *         if {@code "localAddress"} option's value is
         *            neither a {@link SocketAddress} nor {@code null}
         * @throws ChannelPipelineException
         *         if this bootstrap's {@link #setPipelineFactory(ChannelPipelineFactory) pipelineFactory}
         *            failed to create a new {@link ChannelPipeline}
         */
        public IChannelFuture Connect(EndPoint remoteAddress)
        {
            if (remoteAddress == null)
            {
                throw new ArgumentNullException("remoteAddress");
            }
            EndPoint localAddress = (EndPoint)GetOption("localAddress");
            return Connect(remoteAddress, localAddress);
        }

        /**
         * Attempts a new connection with the specified {@code remoteAddress} and
         * the specified {@code localAddress}.  If the specified local address is
         * {@code null}, the local address of a new channel is determined
         * automatically.
         *
         * @return a future object which notifies when this connection attempt
         *         succeeds or fails
         *
         * @throws ChannelPipelineException
         *         if this bootstrap's {@link #setPipelineFactory(ChannelPipelineFactory) pipelineFactory}
         *            failed to create a new {@link ChannelPipeline}
         */
        public IChannelFuture Connect(EndPoint remoteAddress, EndPoint localAddress)
        {

            if (remoteAddress == null)
            {
                throw new ArgumentNullException("remoteAddress");
            }

            IChannelPipeline pipeline;
            try
            {
                pipeline = GetPipelineFactory().GetPipeline();
            }
            catch (Exception e)
            {
                throw new ChannelPipelineException("Failed to initialize a pipeline.", e);
            }

            // Set the options.
            IChannel ch = GetFactory().NewChannel(pipeline);
            bool success = false;
            try
            {
                ch.GetConfig().SetOptions(GetOptions());
                success = true;
            }
            finally
            {
                if (!success)
                {
                    ch.Close();
                }
            }

            // Bind.
            if (localAddress != null)
            {
                ch.Bind(localAddress);
            }

            // Connect.
            return ch.Connect(remoteAddress);
        }

        /**
         * Attempts to bind a channel with the specified {@code localAddress}. later the channel can
         * be connected to a remoteAddress by calling {@link Channel#connect(SocketAddress)}.This method
         * is useful where bind and connect need to be done in separate steps.
         * <p>
         * For an instance, a user can set an attachment to the {@link Channel} via
         * {@link Channel#setAttachment(Object)} before beginning a connection attempt so that the user can access
         * the attachment once the connection is established:
         *
         * <pre>
         *  ChannelFuture bindFuture = bootstrap.bind(new InetSocketAddress("192.168.0.15", 0));
         *  Channel channel = bindFuture.getChannel();
         *  channel.setAttachment(dataObj);
         *  channel.connect(new InetSocketAddress("192.168.0.30", 8080));
         * </pre>
         *
         * The attachment can be accessed then in the handler like the following:
         *
         * <pre>
         *  public class YourHandler extends SimpleChannelUpstreamHandler {
         *      public void channelConnected(ChannelHandlerContext ctx, ChannelStateEvent e) throws Exception {
         *          Object dataObject = ctx.getChannel().getAttachment();
         *      }
         *  }
         *
         * </pre>
         *
         * @return a future object which notifies when this bind attempt
         *         succeeds or fails
         *
         * @throws ChannelPipelineException
         *         if this bootstrap's {@link #setPipelineFactory(ChannelPipelineFactory) pipelineFactory}
         *            failed to create a new {@link ChannelPipeline}
         */
        public IChannelFuture Bind(EndPoint localAddress)
        {

            if (localAddress == null)
            {
                throw new ArgumentNullException("localAddress");
            }

            IChannelPipeline pipeline;
            try
            {
                pipeline = GetPipelineFactory().GetPipeline();
            }
            catch (Exception e)
            {
                throw new ChannelPipelineException("Failed to initialize a pipeline.", e);
            }

            // Set the options.
            IChannel ch = GetFactory().NewChannel(pipeline);
            bool success = false;
            try
            {
                ch.GetConfig().SetOptions(GetOptions());
                success = true;
            }
            finally
            {
                if (!success)
                {
                    ch.Close();
                }
            }

            // Bind.
            return ch.Bind(localAddress);
        }
    }
}
