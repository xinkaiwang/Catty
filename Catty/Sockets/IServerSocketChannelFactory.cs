using Catty.Core.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Sockets
{
    public interface IServerSocketChannelFactory : IServerChannelFactory
    {
        new IServerSocketChannel NewChannel(IChannelPipeline pipeline);
    }
}
