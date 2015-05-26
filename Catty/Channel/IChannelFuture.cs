using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Catty.Core.Channel
{
    //public class IChannelFuture : AnsyncJobV2<IChannel>
    //{
    //}

    public interface IChannelFuture
    {
        /**
         * Returns a channel where the I/O operation associated with this
         * future takes place.
         */
        IChannel GetChannel();

        /**
         * Returns {@code true} if and only if this future is
         * complete, regardless of whether the operation was successful, failed,
         * or cancelled.
         */
        bool IsDone();

        /**
         * Returns {@code true} if and only if this future was
         * cancelled by a {@link #cancel()} method.
         */
        bool IsCancelled();

        /**
         * Returns {@code true} if and only if the I/O operation was completed
         * successfully.
         */
        bool IsSuccess();

        /**
         * Returns the cause of the failed I/O operation if the I/O operation has
         * failed.
         *
         * @return the cause of the failure.
         *         {@code null} if succeeded or this future is not
         *         completed yet.
         */
        Exception GetCause();

        /**
         * Cancels the I/O operation associated with this future
         * and notifies all listeners if canceled successfully.
         *
         * @return {@code true} if and only if the operation has been canceled.
         *         {@code false} if the operation can't be canceled or is already
         *         completed.
         */
        bool Cancel();

        /**
         * Marks this future as a success and notifies all
         * listeners.
         *
         * @return {@code true} if and only if successfully marked this future as
         *         a success. Otherwise {@code false} because this future is
         *         already marked as either a success or a failure.
         */
        bool SetSuccess();

        /**
         * Marks this future as a failure and notifies all
         * listeners.
         *
         * @return {@code true} if and only if successfully marked this future as
         *         a failure. Otherwise {@code false} because this future is
         *         already marked as either a success or a failure.
         */
        bool SetFailure(Exception cause);

        ///**
        // * Notifies the progress of the operation to the listeners that implements
        // * {@link ChannelFutureProgressListener}. Please note that this method will
        // * not do anything and return {@code false} if this future is complete
        // * already.
        // *
        // * @return {@code true} if and only if notification was made.
        // */
        //bool SetProgress(long amount, long current, long total);

        /**
         * Adds the specified listener to this future.  The
         * specified listener is notified when this future is
         * {@linkplain #isDone() done}.  If this future is already
         * completed, the specified listener is notified immediately.
         */
        void AddListener(Action<IChannelFuture> listener);

        /**
         * Removes the specified listener from this future.
         * The specified listener is no longer notified when this
         * future is {@linkplain #isDone() done}.  If the specified
         * listener is not associated with this future, this method
         * does nothing and returns silently.
         */
        void RemoveListener(Action<IChannelFuture> listener);

        ///**
        // * Waits for this future until it is done, and rethrows the cause of the failure if this future
        // * failed.  If the cause of the failure is a checked exception, it is wrapped with a new
        // * {@link ChannelException} before being thrown.
        // */
        //IChannelFuture Sync();

        ///**
        // * Waits for this future until it is done, and rethrows the cause of the failure if this future
        // * failed.  If the cause of the failure is a checked exception, it is wrapped with a new
        // * {@link ChannelException} before being thrown.
        // */
        //IChannelFuture SyncUninterruptibly();

        ///**
        // * Waits for this future to be completed.
        // *
        // * @throws InterruptedException
        // *         if the current thread was interrupted
        // */
        //IChannelFuture Await();

        ///**
        // * Waits for this future to be completed without
        // * interruption.  This method catches an {@link InterruptedException} and
        // * discards it silently.
        // */
        //IChannelFuture AwaitUninterruptibly();

        ///**
        // * Waits for this future to be completed within the
        // * specified time limit.
        // *
        // * @return {@code true} if and only if the future was completed within
        // *         the specified time limit
        // *
        // * @throws InterruptedException
        // *         if the current thread was interrupted
        // */
        //bool Await(long timeoutInMs);

        ///**
        // * Waits for this future to be completed within the
        // * specified time limit without interruption.  This method catches an
        // * {@link InterruptedException} and discards it silently.
        // *
        // * @return {@code true} if and only if the future was completed within
        // *         the specified time limit
        // */
        //bool AwaitUninterruptibly(long timeoutInMs);
    }

    public static class IChannelFutureExt
    {
        // helper function when you need a sync wait
        public static IChannelFuture AwaitUninterruptibly(this IChannelFuture future)
        {
            ManualResetEvent signal = new ManualResetEvent(false);
            future.AddListener(f => signal.Set());
            signal.WaitOne();
            return future;
        }
    }
}
