using Catty.Core.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catty.Core.Sockets.Nio
{
    public class AbstractNioChannelSink : AbstractChannelSink
    {
        public override IChannelFuture Execute(IChannelPipeline pipeline, Action task)
        {
            IChannel ch = pipeline.GetChannel();
            if (ch is NioSocketChannel)
            {
                NioSocketChannel channel = (NioSocketChannel)ch;
                ChannelRunnableWrapper wrapper = new ChannelRunnableWrapper(pipeline.GetChannel(), task);
                channel.ExecuteInIoThread(wrapper);
                return wrapper;
            }
            return base.Execute(pipeline, task);
        }

        //protected bool IsFireExceptionCaughtLater(IChannelEvent eve, Exception actualCause) {
        //    IChannel channel = eve.GetChannel();
        //    bool fireLater = false;
        //    if (channel is NioSocketChannel) {
        //        fireLater =  !AbstractNioWorker.isIoThread((AbstractNioChannel<?>) channel);
        //    }
        //    return fireLater;
        //}

        public override void EventSunk(IChannelPipeline pipeline, IChannelEvent e)
        {
            throw new NotImplementedException();
        }

    }
}
