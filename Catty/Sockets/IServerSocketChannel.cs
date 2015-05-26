using Catty.Core.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Catty.Core.Sockets
{
    public interface IServerSocketChannel : IServerChannel
    {
        IServerSocketChannelConfig GetServerSocketChannelConfig();
        EndPoint GetLocalSocketAddress();
        EndPoint GetRemoteSocketAddress();
    }
}
