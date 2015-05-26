using Catty.Core.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Catty.Core.Sockets.Nio
{
    public class NioClientSocketChannel : NioSocketChannel
    {
        private static Socket NewSocket()
        {
            Socket socket;
            try
            {
                socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            }
            catch (Exception e)
            {
                throw new ChannelException("Failed to open a socket.", e);
            }

            bool success = false;
            try
            {
                //socket.configureBlocking(false);
                success = true;
            }
            catch (Exception e)
            {
                throw new ChannelException("Failed to enter non-blocking mode.", e);
            }
            finally
            {
                if (!success)
                {
                    try
                    {
                        socket.Close();
                    }
                    catch (Exception e)
                    {
                        //if (logger.isWarnEnabled())
                        //{
                        //    logger.warn(
                        //            "Failed to close a partially initialized socket.",
                        //            e);
                        //}
                    }
                }
            }

            return socket;
        }

        volatile IChannelFuture connectFuture;
        volatile bool boundManually;

        // Does not need to be volatile as it's accessed by only one thread.
        long connectDeadlineNanos;
        volatile SocketAddress requestedRemoteAddress;

        internal volatile Timer timoutTimer;

        NioClientSocketChannel(
                IChannelFactory factory, IChannelPipeline pipeline,
                IChannelSink sink)
            : base(null, factory, pipeline, sink, NewSocket())
        {
            Channels.FireChannelOpen(this);
        }
    }
}
