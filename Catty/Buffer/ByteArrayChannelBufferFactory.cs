using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Buffer
{
    public class ByteArrayChannelBufferFactory : IChannelBufferFactory
    {
        private static readonly ByteArrayChannelBufferFactory instance = new ByteArrayChannelBufferFactory();
        public static IChannelBufferFactory GetInstance()
        {
            return instance;
        }

        public IChannelBuffer GetBuffer(int capacity)
        {
            return new ByteArrayChannelBuffer(capacity);
        }
    }

}
