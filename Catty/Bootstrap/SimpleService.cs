using Catty.Core.Bootstrap;
using Catty.Core.Channel;
using Catty.Core.Sockets.Nio;
using Catty.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Bootstrap
{
    public class SimpleTcpService
    {
        private ServerBootstrap bootstrap;

        public SimpleTcpService SetHandlers(Func<IChannelHandler[]> handlersFactory)
        {
            // Configure the server.
            var factory = new NioServerSocketChannelFactory();
            bootstrap = new ServerBootstrap(factory);

            // Set up the pipeline factory.
            bootstrap.SetPipelineFactory(handlersFactory);
            return this;
        }

        public SimpleTcpService Bind(IPEndPoint addr)
        {
            var result = bootstrap.Bind(addr.Serialize());
            return this;
        }

        public SimpleTcpService Bind(int port)
        {
            return Bind(new IPEndPoint(IPAddress.Any, port));
        }
    }
}
