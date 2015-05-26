using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Buffer
{
    public class ByteArrayChannelBuffer : AbstractChannelBuffer
    {
        internal byte[] bytes;
        internal int sliceStart;
        internal int sliceEnd;
        public ByteArrayChannelBuffer(int capacity)
        {
            int estimate = CalulateBufferSize(capacity);
            this.bytes = new byte[estimate];
            this.sliceStart = 0;
            this.sliceEnd = bytes.Length;
        }

        private static int CalulateBufferSize(int reqired)
        {
            int estimate = 64;
            while (estimate < reqired)
            {
                estimate <<= 1;
                if (estimate == 0)
                    throw new InvalidOperationException("reqired size exceded 2G?");
            }
            return estimate;
        }

        // don't call this directly, please call Slice() instead
        private ByteArrayChannelBuffer(ByteArrayChannelBuffer parent, int index, int length)
        {
            this.bytes = parent.bytes;
            this.sliceStart = parent.sliceStart + index;
            this.sliceEnd = parent.sliceStart + index + length;
            this.SetIndex(0, length); // now if you look at this buffer instance, you are only going to see a sub-set of the data (slice). start at 0, end at "length".
        }

        public override IChannelBufferFactory Factory()
        {
            return ByteArrayChannelBufferFactory.GetInstance();
        }

        public override int Capacity
        {
            get
            {
                return sliceEnd - sliceStart;
            }
        }

        public override byte GetByte(int index)
        {
            return bytes[sliceStart + index];
        }

        public override void GetBytes(int index, Action<byte[], int, int> visitor, int length)
        {
            visitor(this.bytes, sliceStart + index, length);
        }

        public override IChannelBuffer SetByte(int index, byte value)
        {
            this.bytes[sliceStart + index] = value;
            return this;
        }

        public override IChannelBuffer SetBytes(int index, byte[] src, int srcIndex, int length)
        {
            Array.Copy(src, srcIndex, this.bytes, sliceStart + index, length);
            return this;
        }

        public override IChannelBuffer SetZero(int index, int length)
        {
            Array.Clear(this.bytes, sliceStart + index, length);
            return this;
        }

        public override IChannelBuffer Copy(int index, int length)
        {
            IChannelBuffer newBuf = Factory().GetBuffer(length);
            this.GetBytes(index, newBuf, 0, length);
            return newBuf;
        }
        public override IChannelBuffer Slice(int index, int length)
        {
            return new ByteArrayChannelBuffer(this, index, length);
        }

        public override IChannelBuffer Duplicate()
        {
            return new ByteArrayChannelBuffer(this, 0, Capacity);
        }
    }

}
