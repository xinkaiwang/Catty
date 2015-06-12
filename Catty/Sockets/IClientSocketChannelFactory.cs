using Catty.Core.Channel;
using Catty.Core.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catty.Sockets.Nio
{
    public interface IClientSocketChannelFactory : IChannelFactory
    {
        ISocketChannel NewChannel(IChannelPipeline pipeline);
    }
}
