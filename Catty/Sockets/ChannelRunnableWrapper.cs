using Catty.Core.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Sockets
{
    public class ChannelRunnableWrapper : DefaultChannelFuture
    {
        private readonly Action task;
        private bool started;

        public ChannelRunnableWrapper(IChannel channel, Action task)
            : base(channel, true)
        {
            this.task = task;
        }

        public void run()
        {
            lock (this)
            {
                if (!IsCancelled())
                {
                    started = true;
                }
                else
                {
                    return;
                }
            }
            try
            {
                task();
                SetSuccess();
            }
            catch (Exception t)
            {
                SetFailure(t);
            }
        }

        public bool Cancel()
        {
            lock (this)
            {
                if (started)
                {
                    return false;
                }
                return base.Cancel();
            }
        }
    }
}
