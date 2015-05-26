using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Buffer
{
    public interface IChannelBuffer
    {
        IChannelBufferFactory Factory();

        // Returns the number of bytes (octets) this buffer can contain.
        int Capacity { get; }

        int WriterIndex { get; set; }

        int ReaderIndex { get; set; }

        // return this
        IChannelBuffer SetIndex(int readerIndex, int writerIndex);

        int ReadableBytes { get; }
        int WritableBytes { get; }

        // Set both readerIndex and writerIndex to 0
        void Clear();

        // return this
        // Discards the bytes between the 0th index and the readerIndex
        IChannelBuffer DiscardReadBytes();

        // return this
        // Ensure we have enough space to write these bytes. (may need enlarge buffer)
        IChannelBuffer EnsureWritableBytes(int writableBytes);

        // read byte in the specified location. this will not modify readerIndex or writerIndex
        byte GetByte(int index);
        void GetBytes(int index, byte[] dst, int dstIndex, int length);
        void GetBytes(int index, IChannelBuffer dst, int dstIndex, int length);
        void GetBytes(int index, Action<byte[], int, int> visitor, int length);

        // read int32 in the specified location. this will not modify readerIndex or writerIndex
        Int32 GetInt32BigEndian(int index);
        Int32 GetInt32LittleEndian(int index);

        IChannelBuffer SetByte(int index, byte value);
        IChannelBuffer SetBytes(int index, byte[] src, int srcIndex, int length);
        IChannelBuffer SetBytes(int index, IChannelBuffer src, int srcIndex, int length);
        IChannelBuffer SetInt32BigEndian(int index, int value);
        IChannelBuffer SetInt32LittleEndian(int index, int value);

        // return this
        IChannelBuffer SetZero(int index, int length);

        byte ReadByte();
        Int32 ReadInt32BigEndian();
        Int32 ReadInt32LittleEndian();

        // return a new instance of IChannelBuffer, the readerIndex += length
        // and the returned new ChannelBuffer start with 0 , with length
        IChannelBuffer ReadBytes(int length);

        // return this
        IChannelBuffer ReadBytes(IChannelBuffer dst, int length);
        // return this
        IChannelBuffer ReadBytes(IChannelBuffer dst, int dstIndex, int length);
        // return this
        IChannelBuffer ReadBytes(byte[] dst);
        // return this
        IChannelBuffer ReadBytes(byte[] dst, int dstIndex, int length);
        // return this
        IChannelBuffer ReadBytes(Action<byte[], int, int> visitor, int length);

        // return this
        IChannelBuffer SkipBytes(int length);

        // return this
        IChannelBuffer WriteInt32BigEndian(byte value);
        // return this
        IChannelBuffer WriteInt32LittleEndian(byte value);
        // return this
        IChannelBuffer WriteByte(byte value);
        // return this
        IChannelBuffer WriteBytes(byte[] src, int srcIndex, int length);
        // return this
        IChannelBuffer WriteBytes(IChannelBuffer src, int srcIndex, int length);

        // Returns a copy of this buffer's sub-region.  Modifying the content of
        // the returned buffer or this buffer does not affect each other at all
        IChannelBuffer Copy(int index, int length);
        // return a new instance of IChannelBuffer, new buffer index from 0. and any write to new buffer will reflect into old buffer.
        IChannelBuffer Slice(int index, int length);
        // return a new instance of IChannelBuffer, new buffer have it's own readerIndex/writerIndex. and any write to new buffer will reflect into old buffer.
        IChannelBuffer Duplicate();

        String FullDataString { get; } // most likely for bugging use only
    }
}
