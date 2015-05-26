using Catty.Core.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Sockets.Nio
{
    public class NioAcceptedSocketChannel : NioSocketChannel
    {
        internal NioAcceptedSocketChannel(
                IChannelFactory factory, IChannelPipeline pipeline,
                IChannel parent, IChannelSink sink,
                Socket socket)
            : base(parent, factory, pipeline, sink, socket)
        {

            SetConnected();

            Channels.FireChannelOpen(this);
        }
    }
}
