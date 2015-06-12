using Catty.Core.Channel;
using Catty.Core.Sockets;
using Catty.Core.Sockets.Nio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Sockets.Nio
{
    public class NioClientSocketChannelFactory : IClientSocketChannelFactory
    {
        private readonly NioClientSocketPipelineSink sink;
        private bool releasePools;

        public NioClientSocketChannelFactory()
        {
            sink = new NioClientSocketPipelineSink();
        }

        ISocketChannel IClientSocketChannelFactory.NewChannel(IChannelPipeline pipeline)
        {
            NioClientSocketChannel channel = new NioClientSocketChannel(this, pipeline, sink);
            return channel;
        }

        IChannel IChannelFactory.NewChannel(IChannelPipeline pipeline)
        {
            NioClientSocketChannel channel = new NioClientSocketChannel(this, pipeline, sink);
            return channel;
        }

        void Core.Channel.IChannelFactory.Shutdown()
        {
            // nothing to do?
        }
    }
}
