using Catty.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Buffer
{
    public class ChannelBuffers
    {
        public static readonly IChannelBuffer EMPTY_BUFFER = new ByteArrayChannelBuffer(0);

        public static IChannelBuffer Buffer(int capacity)
        {
            return new ByteArrayChannelBuffer(capacity);
        }

        public static IChannelBuffer DynamicBuffer(int capacity)
        {
            return new DynamicChannelBuffer(capacity);
        }

        /**
         * Creates a new buffer which wraps the specified buffer's readable bytes.
         * A modification on the specified buffer's content will be visible to the
         * returned buffer.
         */
        public static IChannelBuffer WrappedBuffer(IChannelBuffer buffer)
        {
            if (buffer.Readable())
            {
                return buffer.Slice();
            }
            else
            {
                return EMPTY_BUFFER;
            }
        }

        public static IChannelBuffer WrappedBuffer(IList<IChannelBuffer> list)
        {
            if (list.Count > 0)
            {
                var newList = list.SelectMany(o =>
                {
                    IEnumerable<IChannelBuffer> ret = null;
                    if (o.ReadableBytes == 0)
                    {
                        ret = new IChannelBuffer[0];
                    }
                    else if (o is CompositeChannelBuffer)
                    {
                        ret = ((CompositeChannelBuffer)o).decompose(o.ReaderIndex, o.ReadableBytes);
                    }
                    else
                    {
                        ret = new IChannelBuffer[] { o.Slice() };
                    }
                    return ret;
                }).ToList();
                return new CompositeChannelBuffer(newList);
            }
            else
            {
                return EMPTY_BUFFER;
            }
        }

        public static IChannelBuffer BufferFromHex(string str)
        {
            byte[] data = BytesEncoder.DecodeBytesFromString(str);
            var buf = new ByteArrayChannelBuffer(data.Length);
            buf.WriteBytes(data, 0, data.Length);
            return buf;
        }
        public static IChannelBuffer BufferFromString(string str)
        {
            byte[] data = DataTypeString.BytesFromString(str);
            var buf = new ByteArrayChannelBuffer(data.Length);
            buf.WriteBytes(data, 0, data.Length);
            return buf;
        }
    }
}
