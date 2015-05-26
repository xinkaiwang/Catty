using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Buffer
{
    public class DynamicChannelBuffer : AbstractChannelBuffer
    {
        private IChannelBuffer buffer;
        private IChannelBufferFactory factory;

        public DynamicChannelBuffer(int estimatedLength)
            :this(estimatedLength, ByteArrayChannelBufferFactory.GetInstance())
        {
        }

        public DynamicChannelBuffer(int estimatedLength, IChannelBufferFactory factory)
        {
            if (estimatedLength < 0)
            {
                throw new IndexOutOfRangeException("estimatedLength: " + estimatedLength);
            }
            if (factory == null)
            {
                throw new NullReferenceException("factory");
            }
            this.factory = factory;
            buffer = factory.GetBuffer(estimatedLength);
        }

        public override IChannelBufferFactory Factory()
        {
            return this.factory;
        }

        public override IChannelBuffer EnsureWritableBytes(int minWritableBytes)
        {
            if (minWritableBytes <= WritableBytes)
            {
                return this;
            }

            int newCapacity;
            if (Capacity == 0)
            {
                newCapacity = 1;
            }
            else
            {
                newCapacity = Capacity;
            }
            int minNewCapacity = WriterIndex + minWritableBytes;
            while (newCapacity < minNewCapacity)
            {
                newCapacity <<= 1;

                // Check if we exceeded the maximum size of 2gb if this is the case then
                // newCapacity == 0
                if (newCapacity == 0)
                {
                    throw new InvalidOperationException("Maximum size of 2gb exceeded");
                }
            }

            IChannelBuffer newBuffer = factory.GetBuffer(newCapacity);
            newBuffer.WriteBytes(buffer, 0, WriterIndex);
            buffer = newBuffer;
            return this;
        }

        public override int Capacity
        {
            get { return buffer.Capacity; }
        }

        public override byte GetByte(int index)
        {
            return buffer.GetByte(index);
        }

        public override void GetBytes(int index, Action<byte[], int, int> visitor, int length)
        {
            buffer.GetBytes(index, visitor, length);
        }

        public override IChannelBuffer SetByte(int index, byte value)
        {
            buffer.SetByte(index, value);
            return this;
        }

        public override IChannelBuffer SetBytes(int index, byte[] src, int srcIndex, int length)
        {
            buffer.SetBytes(index, src, srcIndex, length);
            return this;
        }

        public override int WriterIndex
        {
            set
            {
                base.WriterIndex = value;
                this.buffer.WriterIndex = value;
            }
        }

        public override int ReaderIndex
        {
            set
            {
                base.ReaderIndex = value;
                this.buffer.ReaderIndex = value;
            }
        }

        public override IChannelBuffer Copy(int index, int length)
        {
            DynamicChannelBuffer copiedBuffer = new DynamicChannelBuffer(Math.Max(length, 64), Factory());
            copiedBuffer.buffer = buffer.Copy(index, length);
            copiedBuffer.SetIndex(0, length);
            return copiedBuffer;
        }

        public override IChannelBuffer Slice(int index, int length)
        {
            return buffer.Slice(index, length);
        }

        public override IChannelBuffer Duplicate()
        {
            return buffer.Duplicate();
        }
    }
}
