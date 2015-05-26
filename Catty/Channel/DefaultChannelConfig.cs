using Catty.Core.Buffer;
using Catty.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Channel
{
    public class DefaultChannelConfig : IChannelConfig
    {
        private volatile IChannelBufferFactory bufferFactory = ByteArrayChannelBufferFactory.GetInstance();
        private volatile int connectTimeoutMillis = 10000; // 10 seconds

        public void SetOptions(Dictionary<String, Object> options)
        {
            foreach (var item in options)
            {
                SetOption(item.Key, item.Value);
            }
        }

        public virtual bool SetOption(String key, Object value)
        {
            if (key == null)
            {
                throw new NullReferenceException("key");
            }

            if ("pipelineFactory".Equals(key))
            {
                SetPipelineFactory((IChannelPipelineFactory)value);
            }
            else if ("connectTimeoutMillis".Equals(key))
            {
                SetConnectTimeoutMillis(ConversionUtil.ToInt(value));
            }
            else if ("bufferFactory".Equals(key))
            {
                SetBufferFactory((IChannelBufferFactory)value);
            }
            else
            {
                return false;
            }
            return true;
        }

        public int GetConnectTimeoutMillis()
        {
            return connectTimeoutMillis;
        }

        public IChannelBufferFactory GetBufferFactory()
        {
            return bufferFactory;
        }

        public void SetBufferFactory(IChannelBufferFactory bufferFactory)
        {
            if (bufferFactory == null)
            {
                throw new NullReferenceException("bufferFactory");
            }
            this.bufferFactory = bufferFactory;
        }

        public IChannelPipelineFactory GetPipelineFactory()
        {
            return null;
        }

        public void SetConnectTimeoutMillis(int connectTimeoutMillis)
        {
            if (connectTimeoutMillis < 0)
            {
                throw new ArgumentException("connectTimeoutMillis: " + connectTimeoutMillis);
            }
            this.connectTimeoutMillis = connectTimeoutMillis;
        }

        public void SetPipelineFactory(IChannelPipelineFactory pipelineFactory)
        {
            // Unused
        }

    }
}
