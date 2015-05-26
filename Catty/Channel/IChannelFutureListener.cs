using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Channel
{
    public class ChannelFutureListeners
    {
        public static void Close(IChannelFuture future)
        {
            future.GetChannel().Close();
        }
    }

    public interface IChannelFutureListener
    {

        void OperationComplete(IChannelFuture future);
    }
}
