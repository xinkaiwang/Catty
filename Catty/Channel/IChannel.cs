using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Catty.Core.Channel
{
    public static class ChannelValues
    {
        /**
         * The {@link #getInterestOps() interestOps} value which tells that only
         * read operation has been suspended.
         */
        public const int OP_NONE = 0;

        /**
         * The {@link #getInterestOps() interestOps} value which tells that neither
         * read nor write operation has been suspended.
         */
        public const int OP_READ = 1;

        /**
         * The {@link #getInterestOps() interestOps} value which tells that both
         * read and write operation has been suspended.
         */
        public const int OP_WRITE = 4;

        /**
         * The {@link #getInterestOps() interestOps} value which tells that only
         * write operation has been suspended.
         */
        public const int OP_READ_WRITE = OP_READ | OP_WRITE;
    }

    public interface IChannel
    {

        /**
         * Returns the unique integer ID of this channel.
         */
        int GetId();

        /**
         * Returns the {@link ChannelFactory} which created this channel.
         */
        IChannelFactory GetFactory();

        /**
         * Returns the parent of this channel.
         *
         * @return the parent channel.
         *         {@code null} if this channel does not have a parent channel.
         */
        IChannel GetParent();

        /**
         * Returns the configuration of this channel.
         */
        IChannelConfig GetConfig();

        /**
         * Returns the {@link ChannelPipeline} which handles {@link ChannelEvent}s
         * associated with this channel.
         */
        IChannelPipeline GetPipeline();

        /**
         * Returns {@code true} if and only if this channel is open.
         */
        bool IsOpen();

        /**
         * Returns {@code true} if and only if this channel is bound to a
         * {@linkplain #getLocalAddress() local address}.
         */
        bool IsBound();

        /**
         * Returns {@code true} if and only if this channel is connected to a
         * {@linkplain #getRemoteAddress() remote address}.
         */
        bool IsConnected();

        /**
         * Returns the local address where this channel is bound to.  The returned
         * {@link SocketAddress} is supposed to be down-cast into more concrete
         * type such as {@link InetSocketAddress} to retrieve the detailed
         * information.
         *
         * @return the local address of this channel.
         *         {@code null} if this channel is not bound.
         */
        EndPoint GetLocalAddress();

        /**
         * Returns the remote address where this channel is connected to.  The
         * returned {@link SocketAddress} is supposed to be down-cast into more
         * concrete type such as {@link InetSocketAddress} to retrieve the detailed
         * information.
         *
         * @return the remote address of this channel.
         *         {@code null} if this channel is not connected.
         *         If this channel is not connected but it can receive messages
         *         from arbitrary remote addresses (e.g. {@link DatagramChannel},
         *         use {@link MessageEvent#getRemoteAddress()} to determine
         *         the origination of the received message as this method will
         *         return {@code null}.
         */
        EndPoint GetRemoteAddress();

        /**
         * Sends a message to this channel asynchronously.    If this channel was
         * created by a connectionless transport (e.g. {@link DatagramChannel})
         * and is not connected yet, you have to call {@link #write(Object, SocketAddress)}
         * instead.  Otherwise, the write request will fail with
         * {@link NotYetConnectedException} and an {@code 'exceptionCaught'} event
         * will be triggered.
         *
         * @param message the message to write
         *
         * @return the {@link ChannelFuture} which will be notified when the
         *         write request succeeds or fails
         *
         * @throws NullPointerException if the specified message is {@code null}
         */
        IChannelFuture Write(Object message);

        /**
         * Sends a message to this channel asynchronously.  It has an additional
         * parameter that allows a user to specify where to send the specified
         * message instead of this channel's current remote address.  If this
         * channel was created by a connectionless transport (e.g. {@link DatagramChannel})
         * and is not connected yet, you must specify non-null address.  Otherwise,
         * the write request will fail with {@link NotYetConnectedException} and
         * an {@code 'exceptionCaught'} event will be triggered.
         *
         * @param message       the message to write
         * @param remoteAddress where to send the specified message.
         *                      This method is identical to {@link #write(Object)}
         *                      if {@code null} is specified here.
         *
         * @return the {@link ChannelFuture} which will be notified when the
         *         write request succeeds or fails
         *
         * @throws NullPointerException if the specified message is {@code null}
         */
        IChannelFuture Write(Object message, EndPoint remoteAddress);

        /**
         * Binds this channel to the specified local address asynchronously.
         *
         * @param localAddress where to bind
         *
         * @return the {@link ChannelFuture} which will be notified when the
         *         bind request succeeds or fails
         *
         * @throws NullPointerException if the specified address is {@code null}
         */
        IChannelFuture Bind(EndPoint localAddress);

        /**
         * Connects this channel to the specified remote address asynchronously.
         *
         * @param remoteAddress where to connect
         *
         * @return the {@link ChannelFuture} which will be notified when the
         *         connection request succeeds or fails
         *
         * @throws NullPointerException if the specified address is {@code null}
         */
        IChannelFuture Connect(EndPoint remoteAddress);

        /**
         * Disconnects this channel from the current remote address asynchronously.
         *
         * @return the {@link ChannelFuture} which will be notified when the
         *         disconnection request succeeds or fails
         */
        IChannelFuture Disconnect();

        /**
         * Unbinds this channel from the current local address asynchronously.
         *
         * @return the {@link ChannelFuture} which will be notified when the
         *         unbind request succeeds or fails
         */
        IChannelFuture Unbind();

        /**
         * Closes this channel asynchronously.  If this channel is bound or
         * connected, it will be disconnected and unbound first.  Once a channel
         * is closed, it can not be open again.  Calling this method on a closed
         * channel has no effect.  Please note that this method always returns the
         * same future instance.
         *
         * @return the {@link ChannelFuture} which will be notified when the
         *         close request succeeds or fails
         */
        IChannelFuture Close();

        /**
         * Returns the {@link ChannelFuture} which will be notified when this
         * channel is closed.  This method always returns the same future instance.
         */
        IChannelFuture GetCloseFuture();

        /**
         * Returns the current {@code interestOps} of this channel.
         *
         * @return {@link #OP_NONE}, {@link #OP_READ}, {@link #OP_WRITE}, or
         *         {@link #OP_READ_WRITE}
         */
        int GetInterestOps();

        /**
         * Returns {@code true} if and only if the I/O thread will read a message
         * from this channel.  This method is a shortcut to the following code:
         * <pre>
         * return (getInterestOps() & OP_READ) != 0;
         * </pre>
         */
        bool IsReadable();

        /**
         * Returns {@code true} if and only if the I/O thread will perform the
         * requested write operation immediately.  Any write requests made when
         * this method returns {@code false} are queued until the I/O thread is
         * ready to process the queued write requests.  This method is a shortcut
         * to the following code:
         * <pre>
         * return (getInterestOps() & OP_WRITE) == 0;
         * </pre>
         */
        bool IsWritable();

        /**
         * Changes the {@code interestOps} of this channel asynchronously.
         *
         * @param interestOps the new {@code interestOps}
         *
         * @return the {@link ChannelFuture} which will be notified when the
         *         {@code interestOps} change request succeeds or fails
         */
        IChannelFuture SetInterestOps(int interestOps);

        /**
         * Suspends or resumes the read operation of the I/O thread asynchronously.
         * This method is a shortcut to the following code:
         * <pre>
         * int interestOps = getInterestOps();
         * if (readable) {
         *     setInterestOps(interestOps | OP_READ);
         * } else {
         *     setInterestOps(interestOps & ~OP_READ);
         * }
         * </pre>
         *
         * @param readable {@code true} to resume the read operation and
         *                 {@code false} to suspend the read operation
         *
         * @return the {@link ChannelFuture} which will be notified when the
         *         {@code interestOps} change request succeeds or fails
         */
        IChannelFuture SetReadable(bool readable);

        /**
         * Retrieves an object which is {@link #setAttachment(Object) attached} to
         * this {@link Channel}.
         *
         * @return {@code null} if no object was attached or {@code null} was
         *         attached
         */
        Object GetAttachment();

        /**
         * Attaches an object to this {@link Channel} to store a stateful
         * information
         */
        void SetAttachment(Object attachment);
    }
}
