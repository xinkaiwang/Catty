using Catty.Core.Buffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Channel
{
    public class DefaultServerChannelConfig : IChannelConfig
    {
        private volatile IChannelPipelineFactory pipelineFactory;
        private volatile IChannelBufferFactory bufferFactory = ByteArrayChannelBufferFactory.GetInstance();

        public virtual void SetOptions(Dictionary<String, Object> options)
        {
            foreach (var e in options)
            {
                SetOption(e.Key, e.Value);
            }
        }

        /**
         * Sets an individual option.  You can override this method to support
         * additional configuration parameters.
         */
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

        public IChannelPipelineFactory GetPipelineFactory()
        {
            return pipelineFactory;
        }

        public void SetPipelineFactory(IChannelPipelineFactory pipelineFactory)
        {
            if (pipelineFactory == null)
            {
                throw new NullReferenceException("pipelineFactory");
            }
            this.pipelineFactory = pipelineFactory;
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

        public int GetConnectTimeoutMillis()
        {
            return 0;
        }

        public void SetConnectTimeoutMillis(int connectTimeoutMillis)
        {
            // Unused
        }
    }
}
