using Catty.Core.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Sockets.Nio
{
    public class NioServerSocketChannelFactory : IServerSocketChannelFactory 
    {
        private readonly NioServerSocketPipelineSink sink = new NioServerSocketPipelineSink();
        //private readonly Executor worker = new ThreadPoolExecutor(2);

        public IServerSocketChannel NewChannel(IChannelPipeline pipeline)
        {
            NioServerSocketChannel channel = new NioServerSocketChannel(this, pipeline, sink);
            return channel;
        }

        public void Shutdown()
        {
            // nothing to do? 
            // our ThreadPoolExecutor don't need to release, it do not use any resource when not in use.
        }

        IServerChannel IServerChannelFactory.NewChannel(IChannelPipeline pipeline)
        {
            return NewChannel(pipeline); // IServerSocketChannelFactory
        }

        IChannel IChannelFactory.NewChannel(IChannelPipeline pipeline)
        {
            return NewChannel(pipeline); // IServerSocketChannelFactory
        }
    }
}
