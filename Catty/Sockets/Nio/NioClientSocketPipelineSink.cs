using Catty.Core.Channel;
using Catty.Core.Sockets.Nio;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Sockets.Nio
{
    class NioClientSocketPipelineSink : AbstractNioChannelSink
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(NioClientSocketPipelineSink));

        internal NioClientSocketPipelineSink()
        {
        }

        public override void EventSunk(IChannelPipeline pipeline, IChannelEvent e)
        {
            if (e is IChannelStateEvent)
            {
                IChannelStateEvent eve = (IChannelStateEvent)e;
                NioClientSocketChannel channel =
                    (NioClientSocketChannel)eve.GetChannel();
                IChannelFuture future = eve.GetFuture();
                ChannelState state = eve.GetState();
                Object value = eve.GetValue();

                switch (state)
                {
                    case ChannelState.OPEN:
                        if (!((bool)value))
                        {
                            channel.Close(channel, future);
                        }
                        break;
                    case ChannelState.BOUND:
                        if (value != null)
                        {
                            Bind(channel, future, (IPEndPoint)value);
                        }
                        else
                        {
                            channel.Close(channel, future);
                        }
                        break;
                    case ChannelState.CONNECTED:
                        if (value != null)
                        {
                            connect(channel, future, (IPEndPoint)value);
                        }
                        else
                        {
                            channel.Close(channel, future);
                        }
                        break;
                    case ChannelState.INTEREST_OPS:
                        channel.SetInterestOps((int)value);
                        break;
                }
            }
            else if (e is IMessageEvent)
            {
                IMessageEvent eve = (IMessageEvent)e;
                NioSocketChannel channel = (NioSocketChannel)eve.GetChannel();
                channel.WriteIntoSocket(eve);
            }
        }

        private static void Bind(NioClientSocketChannel channel, IChannelFuture future, EndPoint localAddress)
        {
            try
            {
                channel.Bind(localAddress);
                channel.boundManually = true;
                channel.SetBound();
                future.SetSuccess();
                Channels.FireChannelBound(channel, channel.GetLocalAddress());
            }
            catch (Exception e)
            {
                future.SetFailure(e);
                Channels.FireExceptionCaught(channel, e);
            }
        }

        private void connect(NioClientSocketChannel channel, IChannelFuture cf, IPEndPoint remoteAddress)
        {
            try
            {
                channel.Connect(cf, remoteAddress);
            }
            catch (Exception t)
            {
                cf.SetFailure(t);
                Channels.FireExceptionCaught(channel, t);
                channel.socket.Close();
            }
        }

    }
}
