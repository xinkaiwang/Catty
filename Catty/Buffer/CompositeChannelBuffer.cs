using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Buffer
{
    /**
     * A virtual buffer which shows multiple buffers as a single merged buffer.  It
     * is recommended to use {@link ChannelBuffers#wrappedBuffer(ChannelBuffer...)}
     * instead of calling the constructor explicitly.
     */
    public class CompositeChannelBuffer : AbstractChannelBuffer
    {
        private IChannelBuffer[] components;
        // start index of each components
        private int[] indices;

        // componentId
        private int lastAccessedComponentId;

        public CompositeChannelBuffer(List<IChannelBuffer> buffers)
        {
            setComponents(buffers);
        }

        public List<IChannelBuffer> decompose(int index, int length)
        {
            if (length == 0)
            {
                return new List<IChannelBuffer>();
            }

            if (index + length > Capacity)
            {
                throw new IndexOutOfRangeException("Too many bytes to decompose - Need "
                        + (index + length) + ", capacity is " + Capacity);
            }

            int componentId = ComponentIdFromIndex(index);
            List<IChannelBuffer> slice = new List<IChannelBuffer>(components.Length);

            // The first component
            IChannelBuffer first = components[componentId].Duplicate();
            first.ReaderIndex = index - indices[componentId];

            IChannelBuffer buf = first;
            int bytesToSlice = length;
            do
            {
                int readableBytes = buf.ReadableBytes;
                if (bytesToSlice <= readableBytes)
                {
                    // Last component
                    buf.WriterIndex = buf.ReaderIndex + bytesToSlice;
                    slice.Add(buf);
                    break;
                }
                else
                {
                    // Not the last component
                    slice.Add(buf);
                    bytesToSlice -= readableBytes;
                    componentId++;

                    // Fetch the next component.
                    buf = components[componentId].Duplicate();
                }
            } while (bytesToSlice > 0);

            // Slice all components because only readable bytes are interesting.
            slice = slice.Select(o => o.Slice()).ToList();

            return slice;
        }

        private void setComponents(List<IChannelBuffer> newComponents)
        {
            //assert !newComponents.isEmpty();

            // Clear the cache.
            lastAccessedComponentId = 0;

            // Build the component array.
            components = newComponents.ToArray();

            // Build the component lookup table.
            indices = new int[components.Length + 1];
            indices[0] = 0;
            for (int i = 1; i <= components.Length; i++)
            {
                indices[i] = indices[i - 1] + components[i - 1].Capacity;
            }

            // Reset the indexes.
            SetIndex(0, Capacity);
        }

        private int ComponentIdFromIndex(long index)
        {
            if (index > this.Capacity)
                throw new IndexOutOfRangeException();
            int lastComponentId = lastAccessedComponentId;
            if (index >= indices[lastComponentId])
            {
                if (index < indices[lastComponentId + 1])
                {
                    return lastComponentId;
                }

                // Search right
                for (int i = lastComponentId + 1; i < components.Length; i++)
                {
                    if (index < indices[i + 1])
                    {
                        lastAccessedComponentId = i;
                        return i;
                    }
                }
            }
            else
            {
                // Search left
                for (int i = lastComponentId - 1; i >= 0; i--)
                {
                    if (index >= indices[i])
                    {
                        lastAccessedComponentId = i;
                        return i;
                    }
                }
            }

            throw new IndexOutOfRangeException("Invalid index: " + index + ", maximum: " + indices.Length);
        }


        public override int Capacity
        {
            get { return this.indices[this.components.Length]; }
        }

        //public override IChannelBuffer DiscardReadBytes()
        //{
        //}

        public override IChannelBufferFactory Factory()
        {
            return ByteArrayChannelBufferFactory.GetInstance();
        }

        public override byte GetByte(int index)
        {
            int componentId = ComponentIdFromIndex(index);
            return components[componentId].GetByte(index - indices[componentId]);
        }

        public override void GetBytes(int index, Action<byte[], int, int> visitor, int length)
        {
            if (index > Capacity - length)
                throw new IndexOutOfRangeException();
            if (index < 0)
            {
                throw new IndexOutOfRangeException("Index must be >= 0");
            }
            if (length == 0)
            {
                return;
            }
            int i = ComponentIdFromIndex(index);
            while (length > 0)
            {
                IChannelBuffer s = components[i];
                int adjustment = indices[i];
                int localLength = Math.Min(length, s.Capacity - (index - adjustment));
                s.GetBytes(index - adjustment, visitor, localLength);
                index += localLength;
                length -= localLength;
                i++;
            }
        }

        public override IChannelBuffer SetByte(int index, byte value)
        {
            int componentId = ComponentIdFromIndex(index);
            components[componentId].SetByte(index - indices[componentId], value);
            return this;
        }

        public override IChannelBuffer SetBytes(int index, byte[] src, int srcIndex, int length)
        {
            int componentId = ComponentIdFromIndex(index);
            if (index > Capacity - length || srcIndex > src.Length - length)
            {
                throw new IndexOutOfRangeException("Too many bytes to read - needs "
                        + (index + length) + " or " + (srcIndex + length) + ", maximum is "
                        + Capacity + " or " + src.Length);
            }

            int i = componentId;
            while (length > 0)
            {
                IChannelBuffer s = components[i];
                int adjustment = indices[i];
                int localLength = Math.Min(length, s.Capacity - (index - adjustment));
                s.SetBytes(index - adjustment, src, srcIndex, localLength);
                index += localLength;
                srcIndex += localLength;
                length -= localLength;
                i++;
            }
            return this;
        }

        public override IChannelBuffer Copy(int index, int length)
        {
            if (index > Capacity - length)
            {
                throw new IndexOutOfRangeException("Too many bytes to copy - Needs "
                        + (index + length) + ", maximum is " + Capacity);
            }

            IChannelBuffer dst = Factory().GetBuffer(length);
            this.GetBytes(index, dst, 0, length);
            return dst;
        }

        public override IChannelBuffer Slice(int index, int length)
        {
            if (index == 0)
            {
                if (length == 0)
                {
                    return ChannelBuffers.EMPTY_BUFFER;
                }
            }
            else if (index < 0 || index > Capacity - length)
            {
                throw new IndexOutOfRangeException("Invalid index: " + index
                        + " - Bytes needed: " + (index + length) + ", maximum is "
                        + Capacity);
            }
            else if (length == 0)
            {
                return ChannelBuffers.EMPTY_BUFFER;
            }

            List<IChannelBuffer> components = decompose(index, length);
            switch (components.Count)
            {
                case 0:
                    return ChannelBuffers.EMPTY_BUFFER;
                case 1:
                    return components[0];
                default:
                    return new CompositeChannelBuffer(components);
            }
        }

        public override IChannelBuffer Duplicate()
        {
            IChannelBuffer duplicate = new CompositeChannelBuffer(this.components.ToList());
            duplicate.SetIndex(ReaderIndex, WriterIndex);
            return duplicate;
        }

    }
}
