using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catty.Core.Channel
{
    public abstract class CompleteChannelFuture : IChannelFuture
    {
        private readonly IChannel channel;

        /**
         * Creates a new instance.
         *
         * @param channel the {@link Channel} associated with this future
         */
        protected CompleteChannelFuture(IChannel channel)
        {
            if (channel == null)
            {
                throw new NullReferenceException("channel");
            }
            this.channel = channel;
        }

        public void AddListener(Action<IChannelFuture> listener)
        {
            try
            {
                listener(this);
            }
            catch (Exception t)
            {
                //if (logger.isWarnEnabled()) {
                //    logger.warn(
                //            "An exception was thrown by " +
                //            ChannelFutureListener.class.getSimpleName() + '.', t);
                //}
            }
        }

        public void RemoveListener(Action<IChannelFuture> listener)
        {
            // NOOP
        }

        //public IChannelFuture await() {
        //    if (Thread.interrupted()) {
        //        throw new InterruptedException();
        //    }
        //    return this;
        //}

        //public boolean await(long timeout, TimeUnit unit) throws InterruptedException {
        //    if (Thread.interrupted()) {
        //        throw new InterruptedException();
        //    }
        //    return true;
        //}

        //public boolean await(long timeoutMillis) throws InterruptedException {
        //    if (Thread.interrupted()) {
        //        throw new InterruptedException();
        //    }
        //    return true;
        //}

        //public ChannelFuture awaitUninterruptibly() {
        //    return this;
        //}

        //public boolean awaitUninterruptibly(long timeout, TimeUnit unit) {
        //    return true;
        //}

        //public boolean awaitUninterruptibly(long timeoutMillis) {
        //    return true;
        //}

        public IChannel GetChannel()
        {
            return channel;
        }

        public bool IsDone()
        {
            return true;
        }

        public bool SetProgress(long amount, long current, long total)
        {
            return false;
        }

        public bool SetFailure(Exception cause)
        {
            return false;
        }

        public bool SetSuccess()
        {
            return false;
        }

        public bool Cancel()
        {
            return false;
        }

        public bool IsCancelled()
        {
            return false;
        }


        public abstract bool IsSuccess();

        public abstract Exception GetCause();

    }

}
