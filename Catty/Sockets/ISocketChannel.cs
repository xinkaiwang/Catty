using Catty.Core.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Sockets
{
    public interface ISocketChannel : IChannel
    {
        //SocketChannelConfig getConfig();
        EndPoint GetLocalSocketAddress();
        EndPoint GetRemoteSocketAddress();
    }
}
