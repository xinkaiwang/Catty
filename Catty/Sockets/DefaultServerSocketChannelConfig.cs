using Catty.Core.Channel;
using Catty.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Sockets
{
    public class DefaultServerSocketChannelConfig : DefaultServerChannelConfig, IServerSocketChannelConfig
    {
        private readonly Socket socket;
        private volatile int backlog;

        /**
         * Creates a new instance.
         */
        public DefaultServerSocketChannelConfig(Socket socket)
        {
            if (socket == null)
            {
                throw new NullReferenceException("socket");
            }
            this.socket = socket;
        }

        public override bool SetOption(String key, Object value)
        {
            if (base.SetOption(key, value))
            {
                return true;
            }

            if ("receiveBufferSize".Equals(key))
            {
                //SetReceiveBufferSize(ConversionUtil.ToInt(value));
            }
            else if ("reuseAddress".Equals(key))
            {
                SetReuseAddress(ConversionUtil.ToBoolean(value));
            }
            else if ("backlog".Equals(key))
            {
                SetBacklog(ConversionUtil.ToInt(value));
            }
            else
            {
                return false;
            }
            return true;
        }

        public bool IsReuseAddress()
        {
            try
            {
                return (bool)socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress);
            }
            catch (SocketException e)
            {
                throw new ChannelException("IsReuseAddress()", e);
            }
        }

        public void SetReuseAddress(bool reuseAddress)
        {
            try
            {
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, reuseAddress);
            }
            catch (SocketException e)
            {
                throw new ChannelException("SetReuseAddress()", e);
            }
        }

        //public int getReceiveBufferSize() {
        //    try {
        //        return (bool)socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress);
        //    } catch (SocketException e) {
        //        throw new ChannelException(e);
        //    }
        //}

        //public void setReceiveBufferSize(int receiveBufferSize) {
        //    try {
        //        socket.setReceiveBufferSize(receiveBufferSize);
        //    } catch (SocketException e) {
        //        throw new ChannelException(e);
        //    }
        //}

        //public void setPerformancePreferences(int connectionTime, int latency, int bandwidth) {
        //    socket.setPerformancePreferences(connectionTime, latency, bandwidth);
        //}

        public int GetBacklog()
        {
            return backlog;
        }

        public void SetBacklog(int backlog)
        {
            if (backlog < 0)
            {
                throw new ArgumentException("backlog: " + backlog);
            }
            this.backlog = backlog;
        }

    }
}
