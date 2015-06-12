using Catty.Core.Channel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;

namespace Catty.Core.Sockets.Nio
{
    public class NioServerSocketPipelineSink : AbstractNioChannelSink
    {
        public override void EventSunk(
                IChannelPipeline pipeline, IChannelEvent e)
        {
            IChannel channel = e.GetChannel();
            if (channel is NioServerSocketChannel)
            {
                HandleServerSocket(e);
            }
            else if (channel is NioSocketChannel)
            {
                HandleAcceptedSocket(e);
            }
        }

        private static void HandleServerSocket(IChannelEvent e)
        {
            if (!(e is IChannelStateEvent))
            {
                return;
            }

            IChannelStateEvent eve = (IChannelStateEvent)e;
            NioServerSocketChannel channel =
                (NioServerSocketChannel)eve.GetChannel();
            IChannelFuture future = eve.GetFuture();
            ChannelState state = eve.GetState();
            Object value = eve.GetValue();

            switch (state)
            {
                case ChannelState.OPEN:
                    //if (((bool)value) == false) {
                    //    ((NioServerBoss) channel.boss).close(channel, future);
                    //}
                    break;
                case ChannelState.BOUND:
                    //if (value != null) {
                    //    ((NioServerBoss) channel.boss).bind(channel, future, (SocketAddress) value);
                    //} else {
                    //    ((NioServerBoss) channel.boss).close(channel, future);
                    //}
                    {
                        EndPoint localEndPoint = eve.GetValue() as EndPoint;
                        try
                        {
                            channel.BindAndStartAccept(localEndPoint);
                            future.SetSuccess();
                            Channels.FireChannelBound(channel, channel.GetLocalAddress());
                        }
                        catch (Exception ex)
                        {
                            future.SetFailure(ex);
                            Channels.FireExceptionCaught(channel, ex);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private static void HandleAcceptedSocket(IChannelEvent e)
        {
            if (e is IChannelStateEvent)
            {
                IChannelStateEvent eve = (IChannelStateEvent)e;
                NioSocketChannel channel = (NioSocketChannel)eve.GetChannel();
                IChannelFuture future = eve.GetFuture();
                ChannelState state = eve.GetState();
                Object value = eve.GetValue();

                switch (state)
                {
                    case ChannelState.OPEN:
                        if (((bool)value) == false)
                        {
                            channel.Close(channel, future);
                        }
                        break;
                    case ChannelState.BOUND:
                    case ChannelState.CONNECTED:
                        if (value == null)
                        {
                            channel.Close(channel, future);
                        }
                        break;
                    case ChannelState.INTEREST_OPS:
                        //channel.worker.setInterestOps(channel, future, ((Integer) value).intValue());
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
    }
}
