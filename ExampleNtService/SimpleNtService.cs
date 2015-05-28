using Catty.Core.Bootstrap;
using Catty.Core.Channel;
using Catty.Core.Sockets.Nio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ExampleNtService
{
    public class SimpleNtService : ServiceBase
    {
        private ServerBootstrap bootstrap;
        private IPEndPoint addr;
        public SimpleNtService SetPort(IPEndPoint addr)
        {
            this.addr = addr;
            return this;
        }

        public SimpleNtService SetHandlerFactory(Func<IChannelHandler[]> handlersFactory)
        {
            // Configure the server.
            var factory = new NioServerSocketChannelFactory();
            bootstrap = new ServerBootstrap(factory);

            // Set up the pipeline factory.
            bootstrap.SetPipelineFactory(handlersFactory);
            return this;
        }

        protected override void OnStart(string[] args)
        {
            bootstrap.Bind(addr.Serialize());
        }

        protected override void OnStop()
        {
        }
    }
}
