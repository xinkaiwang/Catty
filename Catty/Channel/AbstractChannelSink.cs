using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Channel
{
    public abstract class AbstractChannelSink : IChannelSink
    {
        /**
         * Creates a new instance.
         */
        protected AbstractChannelSink()
        {
        }

        /**
         * Sends an {@link ExceptionEvent} upstream with the specified
         * {@code cause}.
         *
         * @param event the {@link ChannelEvent} which caused a
         *              {@link ChannelHandler} to raise an exception
         * @param cause the exception raised by a {@link ChannelHandler}
         */
        public void ExceptionCaught(IChannelPipeline pipeline,
                IChannelEvent eve, ChannelPipelineException cause)
        {
            Exception actualCause = cause.InnerException;
            if (actualCause == null)
            {
                actualCause = cause;
            }
            if (IsFireExceptionCaughtLater(eve, actualCause))
            {
                Channels.FireExceptionCaughtLater(eve.GetChannel(), actualCause);
            }
            else
            {
                Channels.FireExceptionCaught(eve.GetChannel(), actualCause);
            }
        }

        /**
         * Returns {@code true} if and only if the specified {@code actualCause}, which was raised while
         * handling the specified {@code event}, must trigger an {@code exceptionCaught()} event in
         * an I/O thread.
         *
         * @param event the event which raised exception
         * @param actualCause the raised exception
         */
        protected bool IsFireExceptionCaughtLater(IChannelEvent eve, Exception actualCause)
        {
            return false;
        }

        /**
         * This implementation just directly call {@link Runnable#run()}.
         * Sub-classes should override this if they can handle it in a better way
         */
        public virtual IChannelFuture Execute(IChannelPipeline pipeline, Action task)
        {
            try
            {
                task();
                return Channels.SucceededFuture(pipeline.GetChannel());
            }
            catch (Exception t)
            {
                return Channels.FailedFuture(pipeline.GetChannel(), t);
            }
        }

        public abstract void EventSunk(IChannelPipeline pipeline, IChannelEvent e);

    }
}
