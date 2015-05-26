using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Buffer
{
    public static class ChannelBufferExt
    {
        // Locates the first occurrence of the specified {@code value} in this buffer, 
        // start with current readerIndex (inclusive), end at writerIndex (exclusive).
        // return -1 means not found
        public static int BytesBefore(this IChannelBuffer buf, byte value)
        {
            int length = buf.ReadableBytes;
            int readerIndex = buf.ReaderIndex;
            for (int i = 0; i < length; i++)
            {
                if (buf.GetByte(readerIndex + i) == value)
                {
                    // found
                    return i;
                }
            }
            // not found
            return -1;
        }

        public static IChannelBuffer SetBytes(this IChannelBuffer buf, int index, byte[] src)
        {
            buf.SetBytes(index, src, 0, src.Length);
            return buf;
        }

        public static IChannelBuffer SetBytes(this IChannelBuffer buf, int index, IChannelBuffer src)
        {
            buf.SetBytes(index, src, src.ReadableBytes);
            return buf;
        }

        public static IChannelBuffer SetBytes(this IChannelBuffer buf, int index, IChannelBuffer src, int length)
        {
            if (length > src.ReadableBytes)
            {
                throw new IndexOutOfRangeException("Too many bytes to write: Need "
                        + length + ", maximum is " + src.ReadableBytes);
            }
            buf.SetBytes(index, src, src.ReaderIndex, length);
            src.ReaderIndex = src.ReaderIndex + length;
            return buf;
        }

        public static IChannelBuffer WriteBytes(this IChannelBuffer buf, IChannelBuffer src)
        {
            return WriteBytes(buf, src, src.ReadableBytes);
        }

        public static IChannelBuffer WriteBytes(this IChannelBuffer buf, IChannelBuffer src, int length)
        {
            if (length > src.ReadableBytes)
            {
                throw new IndexOutOfRangeException("Too many bytes to write - Need "
                        + length + ", maximum is " + src.ReadableBytes);
            }
            buf.WriteBytes(src, src.ReaderIndex, length);
            return buf;
        }

        public static bool Readable(this IChannelBuffer buf)
        {
            return buf.ReadableBytes > 0;
        }

        public static bool Writeable(this IChannelBuffer buf)
        {
            return buf.WritableBytes > 0;
        }

        public static IChannelBuffer Copy(this IChannelBuffer buf)
        {
            return buf.Copy(buf.ReaderIndex, buf.ReadableBytes);
        }
        public static IChannelBuffer Slice(this IChannelBuffer buf)
        {
            return buf.Slice(buf.ReaderIndex, buf.ReadableBytes);
        }

        public static byte[] GetByteArray(this IChannelBuffer buf)
        {
            return GetByteArray(buf, buf.ReaderIndex, buf.ReadableBytes);
        }
        public static byte[] GetByteArray(this IChannelBuffer buf, int index, int length)
        {
            byte[] data = new byte[length];
            buf.GetBytes(index, data, 0, length);
            return data;
        }
    }
}
