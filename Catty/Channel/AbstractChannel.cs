using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace Catty.Core.Channel
{
    public abstract class AbstractChannel : IChannel
    {
        //static readonly Dictionary<int, IChannel> allChannels = new ConcurrentHashMap<Integer, Channel>();

        private static readonly Random random = new Random();

        private static int AllocateId(IChannel channel)
        {
            int id = random.Next(int.MaxValue);
            //for (;;) {
            //    // Loop until a unique ID is acquired.
            //    // It should be found in one loop practically.
            //    if (allChannels.putIfAbsent(id, channel) == null) {
            //        // Successfully acquired.
            //        return id;
            //    } else {
            //        // Taken by other channel at almost the same moment.
            //        id = id.intValue() + 1;
            //    }
            //}
            return id;
        }

        /**
         * Creates a new instance.
         *
         * @param parent
         *        the parent of this channel. {@code null} if there's no parent.
         * @param factory
         *        the factory which created this channel
         * @param pipeline
         *        the pipeline which is going to be attached to this channel
         * @param sink
         *        the sink which will receive downstream events from the pipeline
         *        and send upstream events to the pipeline
         */
        protected AbstractChannel(
                IChannel parent, IChannelFactory factory,
                IChannelPipeline pipeline, IChannelSink sink)
        {

            this.parent = parent;
            this.factory = factory;
            this.pipeline = pipeline;
            this.succeededFuture = new SucceededChannelFuture(this);
            this.closeFuture = new ChannelCloseFuture(this);

            id = AllocateId(this);

            pipeline.Attach(this, sink);
        }

        /**
         * (Internal use only) Creates a new temporary instance with the specified
         * ID.
         *
         * @param parent
         *        the parent of this channel. {@code null} if there's no parent.
         * @param factory
         *        the factory which created this channel
         * @param pipeline
         *        the pipeline which is going to be attached to this channel
         * @param sink
         *        the sink which will receive downstream events from the pipeline
         *        and send upstream events to the pipeline
         */
        protected AbstractChannel(
                int id,
                IChannel parent, IChannelFactory factory,
                IChannelPipeline pipeline, IChannelSink sink)
        {

            this.id = id;
            this.parent = parent;
            this.factory = factory;
            this.pipeline = pipeline;
            this.succeededFuture = new SucceededChannelFuture(this);
            this.closeFuture = new ChannelCloseFuture(this);
            pipeline.Attach(this, sink);
        }

        protected readonly int id;
        protected readonly IChannel parent;
        protected readonly IChannelFactory factory;
        protected readonly IChannelPipeline pipeline;
        private readonly IChannelFuture succeededFuture;
        private readonly ChannelCloseFuture closeFuture;
        protected volatile int interestOps = ChannelValues.OP_READ;

        private volatile Object attachment;

        public Object GetAttachment()
        {
            return attachment;
        }

        public void SetAttachment(Object attachment)
        {
            this.attachment = attachment;
        }

        public int GetId()
        {
            return id;
        }

        public IChannel GetParent()
        {
            return parent;
        }

        public IChannelFactory GetFactory()
        {
            return factory;
        }

        public abstract IChannelConfig GetConfig();

        public IChannelPipeline GetPipeline()
        {
            return pipeline;
        }

        /**
         * Returns the cached {@link SucceededChannelFuture} instance.
         */
        public IChannelFuture GetSucceededFuture()
        {
            return succeededFuture;
        }

        /**
         * Returns the {@link FailedChannelFuture} whose cause is an
         * {@link UnsupportedOperationException}.
         */
        protected IChannelFuture GetUnsupportedOperationFuture()
        {
            return new FailedChannelFuture(this, new NotImplementedException());
        }

        /**
         * Returns the ID of this channel.
         */
        //public override int HashCode()
        //{
        //    return id;
        //}

        /**
         * Returns {@code true} if and only if the specified object is identical
         * with this channel (i.e: {@code this == o}).
         */
        public override bool Equals(Object o)
        {
            return this == o;
        }

        /**
         * Compares the {@linkplain #getId() ID} of the two channels.
         */
        public int CompareTo(IChannel o)
        {
            return GetId().CompareTo(o.GetId());
        }

        public bool IsOpen()
        {
            return !closeFuture.IsDone();
        }

        /**
         * Marks this channel as closed.  This method is intended to be called by
         * an internal component - please do not call it unless you know what you
         * are doing.
         *
         * @return {@code true} if and only if this channel was not marked as
         *                      closed yet
         */
        protected virtual bool SetClosed()
        {
            // Deallocate the current channel's ID from allChannels so that other
            // new channels can use it.
            //allChannels.remove(id);

            return closeFuture.SetClosed();
        }

        public IChannelFuture Bind(EndPoint localAddress)
        {
            return Channels.Bind(this, localAddress);
        }

        public IChannelFuture Unbind()
        {
            return Channels.Unbind(this);
        }

        public IChannelFuture Close()
        {
            IChannelFuture returnedCloseFuture = Channels.Close(this);
            Debug.Assert(closeFuture == returnedCloseFuture);
            return closeFuture;
        }

        public IChannelFuture GetCloseFuture()
        {
            return closeFuture;
        }

        public abstract bool IsBound();

        public abstract bool IsConnected();

        public abstract EndPoint GetLocalAddress();

        public abstract EndPoint GetRemoteAddress();

        public abstract int GetInterestOps();

        public bool IsReadable()
        {
            return (GetInterestOps() & ChannelValues.OP_READ) != 0;
        }

        public bool IsWritable()
        {
            return (GetInterestOps() & ChannelValues.OP_WRITE) == 0;
        }

        public IChannelFuture SetReadable(bool readable)
        {
            if (readable)
            {
                return SetInterestOps(GetInterestOps() | ChannelValues.OP_READ);
            }
            else
            {
                return SetInterestOps(GetInterestOps() & ~ChannelValues.OP_READ);
            }
        }

        public abstract IChannelFuture SetInterestOps(int interestOps);

        /**
         * Sets the {@link #getInterestOps() interestOps} property of this channel
         * immediately.  This method is intended to be called by an internal
         * component - please do not call it unless you know what you are doing.
         */
        protected virtual void SetInterestOpsNow(int interestOps)
        {
            this.interestOps = interestOps;
        }

        public override string ToString()
        {
            return this.GetType().Name.Split('.').Last() + "," + this.id;
        }

        private sealed class ChannelCloseFuture : DefaultChannelFuture
        {
            public ChannelCloseFuture(AbstractChannel channel)
                : base(channel, false)
            {
            }

            public override bool SetSuccess()
            {
                // User is not supposed to call this method - ignore silently.
                return false;
            }

            public override bool SetFailure(Exception cause)
            {
                // User is not supposed to call this method - ignore silently.
                return false;
            }

            internal bool SetClosed()
            {
                return base.SetSuccess();
            }
        }


    }
}
