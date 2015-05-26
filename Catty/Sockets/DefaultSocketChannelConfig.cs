using Catty.Core.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Sockets
{
    public class DefaultSocketChannelConfig : DefaultChannelConfig, ISocketChannelConfig
    {
        private readonly Socket socket;
        public DefaultSocketChannelConfig(Socket socket)
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
            else if ("sendBufferSize".Equals(key))
            {
                //SetSendBufferSize(ConversionUtil.ToInt(value));
            }
            else if ("tcpNoDelay".Equals(key))
            {
                //setTcpNoDelay(ConversionUtil.toBoolean(value));
            }
            else if ("keepAlive".Equals(key))
            {
                //setKeepAlive(ConversionUtil.toBoolean(value));
            }
            else if ("reuseAddress".Equals(key))
            {
                //setReuseAddress(ConversionUtil.toBoolean(value));
            }
            else if ("soLinger".Equals(key))
            {
                //setSoLinger(ConversionUtil.toInt(value));
            }
            else if ("trafficClass".Equals(key))
            {
                //setTrafficClass(ConversionUtil.toInt(value));
            }
            else
            {
                return false;
            }
            return true;
        }

        //public int GetSendBufferSize()
        //{
        //    throw new NotImplementedException();
        //}

        //public void SetSendBufferSize(int sendBufferSize)
        //{
        //    throw new NotImplementedException();
        //}

        //public int GetReceiveBufferSize()
        //{
        //    return 0;
        //}

        //public void SetReceiveBufferSize(int receiveBufferSize)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
