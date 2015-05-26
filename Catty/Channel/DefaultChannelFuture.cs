using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Catty.Core.Channel
{
    public class DefaultChannelFuture : IChannelFuture
    {
        private static readonly Exception CANCELLED = new Exception();

        private static volatile bool useDeadLockChecker = true;
        private static bool disabledDeadLockCheckerOnce;

        /**
         * Returns {@code true} if and only if the dead lock checker is enabled.
         */
        public static bool IsUseDeadLockChecker()
        {
            return useDeadLockChecker;
        }

        /**
         * Enables or disables the dead lock checker.  It is not recommended to
         * disable the dead lock checker.  Disable it at your own risk!
         */
        public static void setUseDeadLockChecker(bool useDeadLockChecker)
        {
            if (!useDeadLockChecker && !disabledDeadLockCheckerOnce)
            {
                disabledDeadLockCheckerOnce = true;
                //if (logger.isDebugEnabled()) {
                //    logger.debug(
                //            "The dead lock checker in " +
                //            DefaultChannelFuture.class.getSimpleName() +
                //            " has been disabled as requested at your own risk.");
                //}
            }
            DefaultChannelFuture.useDeadLockChecker = useDeadLockChecker;
        }

        private readonly IChannel channel;
        private readonly bool cancellable;

        private Action<IChannelFuture> firstListener;
        private List<Action<IChannelFuture>> otherListeners;
        private bool done;
        private Exception cause;
        private int waiters;

        /**
         * Creates a new instance.
         *
         * @param channel
         *        the {@link Channel} associated with this future
         * @param cancellable
         *        {@code true} if and only if this future can be canceled
         */
        public DefaultChannelFuture(IChannel channel, bool cancellable)
        {
            this.channel = channel;
            this.cancellable = cancellable;
        }

        public IChannel GetChannel()
        {
            return channel;
        }

        public bool IsDone()
        {
            return done;
        }

        public bool IsSuccess()
        {
            return done && cause == null;
        }

        public virtual Exception GetCause()
        {
            if (cause != CANCELLED)
            {
                return cause;
            }
            else
            {
                return null;
            }
        }

        public bool IsCancelled()
        {
            return cause == CANCELLED;
        }

        public void AddListener(Action<IChannelFuture> listener)
        {
            if (listener == null)
            {
                throw new NullReferenceException("listener");
            }

            bool notifyNow = false;
            lock (this)
            {
                if (done)
                {
                    notifyNow = true;
                }
                else
                {
                    if (firstListener == null)
                    {
                        firstListener = listener;
                    }
                    else
                    {
                        if (otherListeners == null)
                        {
                            otherListeners = new List<Action<IChannelFuture>>(1);
                        }
                        otherListeners.Add(listener);
                    }

                }
            }

            if (notifyNow)
            {
                NotifyListener(listener);
            }
        }

        public void RemoveListener(Action<IChannelFuture> listener)
        {
            if (listener == null)
            {
                throw new NullReferenceException("listener");
            }

            lock (this)
            {
                if (!done)
                {
                    if (listener == firstListener)
                    {
                        if (otherListeners != null && otherListeners.Count() > 0)
                        {
                            firstListener = otherListeners[0];
                            otherListeners.RemoveAt(0);
                        }
                        else
                        {
                            firstListener = null;
                        }
                    }
                    else if (otherListeners != null)
                    {
                        otherListeners.Remove(listener);
                    }

                }
            }
        }


        public virtual bool SetSuccess()
        {
            lock (this)
            {
                // Allow only once.
                if (done)
                {
                    return false;
                }

                done = true;
                if (waiters > 0)
                {
                    NotifyAll();
                }
            }

            NotifyListeners();
            return true;
        }

        public virtual bool SetFailure(Exception cause)
        {
            lock (this)
            {
                // Allow only once.
                if (done)
                {
                    return false;
                }

                this.cause = cause;
                done = true;
                if (waiters > 0)
                {
                    NotifyAll();
                }
            }

            NotifyListeners();
            return true;
        }

        public virtual bool Cancel()
        {
            if (!cancellable)
            {
                return false;
            }

            lock (this)
            {
                // Allow only once.
                if (done)
                {
                    return false;
                }

                cause = CANCELLED;
                done = true;
                if (waiters > 0)
                {
                    NotifyAll();
                }
            }

            NotifyListeners();
            return true;
        }

        private void NotifyAll()
        {
            Monitor.PulseAll(this);
        }
        private void NotifyListeners()
        {
            // This method doesn't need synchronization because:
            // 1) This method is always called after synchronized (this) block.
            //    Hence any listener list modification happens-before this method.
            // 2) This method is called only when 'done' is true.  Once 'done'
            //    becomes true, the listener list is never modified - see add/removeListener()
            if (firstListener != null)
            {
                NotifyListener(firstListener);
                firstListener = null;

                if (otherListeners != null)
                {
                    foreach (var l in otherListeners)
                    {
                        NotifyListener(l);
                    }
                    otherListeners = null;
                }
            }
        }

        private void NotifyListener(Action<IChannelFuture> l)
        {
            try
            {
                l(this);
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

    }
}
