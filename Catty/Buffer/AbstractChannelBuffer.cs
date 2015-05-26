using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Buffer
{
    public abstract class AbstractChannelBuffer : IChannelBuffer
    {
        public abstract IChannelBufferFactory Factory();

        private int readerIndex;
        private int writerIndex;

        public abstract int Capacity { get; }

        public virtual int WriterIndex
        {
            get
            {
                return this.writerIndex;
            }
            set
            {
                if (value < this.readerIndex)
                    throw new IndexOutOfRangeException("current_readerIndex=" + this.readerIndex + " new_writerIndex=" + value);
                if (writerIndex > this.Capacity)
                    throw new IndexOutOfRangeException("current_bytesLength=" + this.Capacity + " new_writerIndex=" + value);
                this.writerIndex = value;
            }
        }

        public virtual int ReaderIndex
        {
            get
            {
                return readerIndex;
            }
            set
            {
                if (value < 0)
                    throw new IndexOutOfRangeException("new_readerIndex=" + value);
                if (value > this.WriterIndex)
                    throw new IndexOutOfRangeException("new_readerIndex=" + value + " current_writerIndex=" + this.writerIndex);
                this.readerIndex = value;
            }
        }


        public IChannelBuffer SetIndex(int readerIndex, int writerIndex)
        {
            this.readerIndex = readerIndex;
            this.writerIndex = writerIndex;
            return this;
        }

        public virtual int ReadableBytes
        {
            get { return this.WriterIndex - this.ReaderIndex; }
        }

        public virtual int WritableBytes
        {
            get { return this.Capacity - this.WriterIndex; }
        }

        public void Clear()
        {
            readerIndex = writerIndex = 0;
        }

        public virtual IChannelBuffer DiscardReadBytes()
        {
            if (readerIndex == 0)
            {
                return this;
            }
            SetBytes(0, this, readerIndex, writerIndex - readerIndex);
            writerIndex -= readerIndex;
            readerIndex = 0;
            return this;
        }

        public virtual IChannelBuffer EnsureWritableBytes(int writableBytes)
        {
            if (writableBytes > this.WritableBytes)
            {
                throw new IndexOutOfRangeException("Writable bytes exceeded: Got "
                        + writableBytes + ", maximum is " + this.WritableBytes);
            }
            return this;
        }

        public abstract byte GetByte(int index);
        public abstract void GetBytes(int index, Action<byte[], int, int> visitor, int length);

        public virtual void GetBytes(int index, byte[] dst, int dstIndex, int length)
        {
            this.GetBytes(index, (src, subStart, subLen) => { Array.Copy(src, subStart, dst, dstIndex, subLen); dstIndex += subLen; }, length);
        }

        public virtual void GetBytes(int index, IChannelBuffer dst, int dstIndex, int length)
        {
            this.GetBytes(index, (src, subStart, subLen) => { dst.SetBytes(dstIndex, src, subStart, subLen); dstIndex += subLen; }, length); 
        }


        public int GetInt32BigEndian(int index)
        {
            byte[] bytes = new byte[4];
            this.GetBytes(index, bytes, 0, 4);
            UInt32 v = ((UInt32)bytes[0] << 24) |
                ((UInt32)bytes[1] << 16) |
                ((UInt32)bytes[2] << 8) |
                ((UInt32)bytes[3]);
            return (int)v;
        }

        public int GetInt32LittleEndian(int index)
        {
            byte[] bytes = new byte[4];
            this.GetBytes(index, bytes, 0, 4);
            UInt32 v = ((UInt32)bytes[3] << 24) |
                ((UInt32)bytes[2] << 16) |
                ((UInt32)bytes[1] << 8) |
                ((UInt32)bytes[0]);
            return (int)v;
        }

        public abstract IChannelBuffer SetByte(int index, byte value);

        public abstract IChannelBuffer SetBytes(int index, byte[] src, int srcIndex, int length);

        public virtual IChannelBuffer SetBytes(int index, IChannelBuffer src, int srcIndex, int length)
        {
            src.GetBytes(srcIndex, (subSrc, subStart, subLen) => { this.SetBytes(index, subSrc, subStart, subLen); index += subLen; }, length);
            return this;
        }

        public virtual IChannelBuffer SetZero(int index, int length)
        {
            if (length < 0)
            {
                throw new IndexOutOfRangeException("length must be 0 or greater than 0.");
            }
            byte[] buf = new byte[1024];
            while (length > 0)
            {
                int localLength = Math.Min(length, buf.Length);
                this.SetBytes(index, buf, 0, localLength);
                index += localLength;
                length -= localLength;
            }
            return this;
        }

        public virtual IChannelBuffer SetInt32BigEndian(int index, int value)
        {
            byte[] bytes = new byte[4];
            bytes[0] = (byte)((value >> 24) & 0xff);
            bytes[1] = (byte)((value >> 16) & 0xff);
            bytes[2] = (byte)((value >> 8) & 0xff);
            bytes[3] = (byte)((value) & 0xff);
            SetBytes(index, bytes, 0, 4);
            return this;
        }

        public virtual IChannelBuffer SetInt32LittleEndian(int index, int value)
        {
            byte[] bytes = new byte[4];
            bytes[3] = (byte)((value >> 24) & 0xff);
            bytes[2] = (byte)((value >> 16) & 0xff);
            bytes[1] = (byte)((value >> 8) & 0xff);
            bytes[0] = (byte)((value) & 0xff);
            SetBytes(index, bytes, 0, 4);
            return this;
        }

        public byte ReadByte()
        {
            if (readerIndex == writerIndex)
            {
                throw new IndexOutOfRangeException("Readable byte limit exceeded: "
                        + readerIndex);
            }
            return GetByte(readerIndex++);
        }

        public int ReadInt32BigEndian()
        {
            int val = this.GetInt32BigEndian(readerIndex);
            readerIndex += 4;
            return val;
        }

        public int ReadInt32LittleEndian()
        {
            int val = this.GetInt32LittleEndian(readerIndex);
            readerIndex += 4;
            return val;
        }

        public IChannelBuffer ReadBytes(byte[] dst)
        {
            return ReadBytes(dst, 0, dst.Length);
        }

        public IChannelBuffer ReadBytes(byte[] dst, int dstIndex, int length)
        {
            CheckReadableBytes(length);
            GetBytes(readerIndex, dst, dstIndex, length);
            readerIndex += length;
            return this;
        }

        public IChannelBuffer ReadBytes(int length)
        {
            CheckReadableBytes(length);
            if (length == 0)
            {
                return null;
            }
            IChannelBuffer buf = this.Factory().GetBuffer(length);
            buf.WriteBytes(this, readerIndex, length);
            readerIndex += length;
            return buf;
        }

        public IChannelBuffer ReadBytes(IChannelBuffer dst, int length)
        {
            if (length > dst.WritableBytes)
            {
                throw new IndexOutOfRangeException("Too many bytes to be read: Need "
                        + length + ", maximum is " + dst.WritableBytes);
            }
            ReadBytes(dst, dst.WriterIndex, length);
            dst.WriterIndex = dst.WriterIndex + length;
            return this;
        }

        public IChannelBuffer ReadBytes(IChannelBuffer dst, int dstIndex, int length)
        {
            CheckReadableBytes(length);
            GetBytes(readerIndex, dst, dstIndex, length);
            readerIndex += length;
            return this;
        }

        public IChannelBuffer ReadBytes(Action<byte[], int, int> visitor, int length)
        {
            GetBytes(readerIndex, visitor, length);
            readerIndex += length;
            return this;
        }

        public IChannelBuffer SkipBytes(int length)
        {
            int newReaderIndex = readerIndex + length;
            if (newReaderIndex > writerIndex)
            {
                throw new IndexOutOfRangeException("Readable bytes exceeded - Need "
                        + newReaderIndex + ", maximum is " + writerIndex);
            }
            readerIndex = newReaderIndex;
            return this;
        }

        public virtual IChannelBuffer WriteInt32BigEndian(byte value)
        {
            EnsureWritableBytes(4);
            this.SetInt32BigEndian(this.writerIndex, value);
            this.writerIndex += 4;
            return this;
        }

        public virtual IChannelBuffer WriteInt32LittleEndian(byte value)
        {
            EnsureWritableBytes(4);
            this.SetInt32LittleEndian(this.writerIndex, value);
            this.writerIndex += 4;
            return this;
        }

        public IChannelBuffer WriteByte(byte value)
        {
            EnsureWritableBytes(1);
            SetByte(writerIndex, value);
            writerIndex++;
            return this;
        }

        public IChannelBuffer WriteBytes(byte[] src, int srcIndex, int length)
        {
            EnsureWritableBytes(length);
            SetBytes(writerIndex, src, srcIndex, length);
            writerIndex += length;
            return this;
        }

        public IChannelBuffer WriteBytes(IChannelBuffer src, int srcIndex, int length)
        {
            EnsureWritableBytes(length);
            SetBytes(writerIndex, src, srcIndex, length);
            writerIndex += length;
            return this;
        }


        public abstract IChannelBuffer Copy(int index, int length);
        public abstract IChannelBuffer Slice(int index, int length);
        public abstract IChannelBuffer Duplicate();

        protected void CheckReadableBytes(int minimumReadableBytes)
        {
            if (this.ReadableBytes < minimumReadableBytes)
            {
                throw new IndexOutOfRangeException("Not enough readable bytes - Need "
                        + minimumReadableBytes + ", maximum is " + this.ReadableBytes);
            }
        }

        public string FullDataString
        {
            get
            {
                var data = this.GetByteArray();
                string tempStr = BitConverter.ToString(data);
                return tempStr.Replace("-", "");
            }
        }

    }
}
