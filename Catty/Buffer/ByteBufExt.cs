﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Buffer
{
    public static class ByteBufExt
    {
        /**
         * Returns the number of readable bytes which is equal to
         * {@code (this.writerIndex - this.readerIndex)}.
         */
        public static int ReadableBytes(this IByteBuf buf)
        {
            return buf.WriterIndex - buf.ReaderIndex;
        }

        /**
         * Returns the number of writable bytes which is equal to
         * {@code (this.capacity - this.writerIndex)}.
         */
        public static int WritableBytes(this IByteBuf buf)
        {
            return buf.Capacity - buf.WriterIndex;
        }

        /**
         * Returns {@code true}
         * if and only if {@code (this.writerIndex - this.readerIndex)} is greater
         * than {@code 0}.
         */
        public static bool IsReadable(this IByteBuf buf)
        {
            return buf.ReadableBytes() > 0;
        }

        /**
         * Returns {@code true} if and only if this buffer contains equal to or more than the specified number of elements.
         */
        public static bool IsReadable(this IByteBuf buf, int size)
        {
            return buf.ReadableBytes() >= size;
        }

        /**
         * Returns {@code true}
         * if and only if {@code (this.capacity - this.writerIndex)} is greater
         * than {@code 0}.
         */
        public static bool IsWritable(this IByteBuf buf)
        {
            return buf.WritableBytes() > 0;
        }

        /**
         * Returns {@code true} if and only if this buffer has enough room to allow writing the specified number of
         * elements.
         */
        public static bool IsWritable(this IByteBuf buf, int size)
        {
            return buf.WritableBytes() >= size;
        }

        /**
         * Gets a boolean at the specified absolute (@code index) in this buffer.
         * This method does not modify the {@code readerIndex} or {@code writerIndex}
         * of this buffer.
         *
         * @throws IndexOutOfBoundsException
         *         if the specified {@code index} is less than {@code 0} or
         *         {@code index + 1} is greater than {@code this.capacity}
         */
        public static bool GetBoolean(this IByteBuf buf, int index)
        {
            byte val = buf.ReadByte();
            return val != 0;
        }

        /**
         * Transfers this buffer's data to the specified destination starting at
         * the specified absolute {@code index}.
         * This method does not modify {@code readerIndex} or {@code writerIndex} of
         * this buffer
         *
         * @throws IndexOutOfBoundsException
         *         if the specified {@code index} is less than {@code 0} or
         *         if {@code index + dst.length} is greater than
         *            {@code this.capacity}
         */
        public static IByteBuf GetBytes(this IByteBuf buf, int index, byte[] dst)
        {
            return buf.GetBytes(index, dst, 0, dst.Length);
        }

        public static IByteBuf GetBytes(this IByteBuf buf, int index, byte[] dst, int dstIndex, int length)
        {
            if (length < 0) throw new IndexOutOfRangeException();
            if (dstIndex + length > dst.Length) throw new IndexOutOfRangeException();
            if (index + length > buf.WriterIndex) throw new IndexOutOfRangeException();
            buf.GetBytes(index, length, (b, start, len) =>
            {
                System.Buffer.BlockCopy(b, start, dst, dstIndex, len);
                dstIndex += len;
                return len;
            });
            return buf;
        }

        /**
         * Transfers the specified source array's data to this buffer starting at
         * the specified absolute {@code index}.
         * This method does not modify {@code readerIndex} or {@code writerIndex} of
         * this buffer.
         *
         * @throws IndexOutOfBoundsException
         *         if the specified {@code index} is less than {@code 0} or
         *         if {@code index + src.length} is greater than
         *            {@code this.capacity}
         */
        public static IByteBuf SetBytes(this IByteBuf buf, int index, byte[] src)
        {
            return buf.SetBytes(index, src, 0, src.Length);
        }

        public static IByteBuf SetBytes(this IByteBuf buf, int index, byte[] src, int srcIndex, int length)
        {
            if (length < 0) throw new IndexOutOfRangeException();
            if (srcIndex + length > src.Length) throw new IndexOutOfRangeException();
            if (index + length > buf.Capacity) throw new IndexOutOfRangeException();
            buf.SetBytes(index, length, (b, start, len) =>
            {
                System.Buffer.BlockCopy(src, srcIndex, b, start, len);
                srcIndex += len;
                return len;
            });
            return buf;
        }

        /**
         * Transfers this buffer's data to the specified destination starting at
         * the current {@code readerIndex} and increases the {@code readerIndex}
         * by the number of the transferred bytes (= {@code dst.length}).
         *
         * @throws IndexOutOfBoundsException
         *         if {@code dst.length} is greater than {@code this.readableBytes}
         */
        public static IByteBuf ReadBytes(this IByteBuf buf, byte[] dst)
        {
            return ReadBytes(buf, buf.ReaderIndex, dst, 0, dst.Length);
        }

        public static IByteBuf ReadBytes(this IByteBuf buf, int index, byte[] dst, int dstIndex, int length)
        {
            if (length < 0) throw new IndexOutOfRangeException();
            if (dstIndex + length > dst.Length) throw new IndexOutOfRangeException();
            if (index + length > buf.WriterIndex) throw new IndexOutOfRangeException();
            buf.GetBytes(index, length, (b, start, len) =>
            {
                System.Buffer.BlockCopy(b, start, dst, dstIndex, len);
                dstIndex += len;
                return len;
            });
            return buf;
        }

        public static IByteBuf WriteBytes(this IByteBuf buf, byte[] src)
        {
            return WriteBytes(buf, src, 0, src.Length);
        }

        public static IByteBuf WriteBytes(this IByteBuf buf, byte[] src, int srcIndex, int length)
        {
            if (length < 0) throw new IndexOutOfRangeException();
            if (srcIndex + length > src.Length) throw new IndexOutOfRangeException();
            if (buf.WriterIndex + length > buf.Capacity) throw new IndexOutOfRangeException();
            buf.WriteBytes(length, (b, start, len) =>
            {
                System.Buffer.BlockCopy(src, srcIndex, b, start, len);
                srcIndex += len;
                return len;
            });
            return buf;
        }

        /**
         * Locates the first occurrence of the specified {@code value} in this
         * buffer.  The search takes place from the current {@code readerIndex}
         * (inclusive) to the current {@code writerIndex} (exclusive).
         * <p>
         * This method does not modify {@code readerIndex} or {@code writerIndex} of
         * this buffer.
         *
         * @return the number of bytes between the current {@code readerIndex}
         *         and the first occurrence if found. {@code -1} otherwise.
         */
        public static int BytesBefore(this IByteBuf buf, byte value)
        {
            return BytesBefore(buf, buf.ReadableBytes(), value);
        }

        /**
         * Locates the first occurrence of the specified {@code value} in this
         * buffer.  The search starts from the current {@code readerIndex}
         * (inclusive) and lasts for the specified {@code length}.
         * <p>
         * This method does not modify {@code readerIndex} or {@code writerIndex} of
         * this buffer.
         *
         * @return the number of bytes between the current {@code readerIndex}
         *         and the first occurrence if found. {@code -1} otherwise.
         *
         * @throws IndexOutOfBoundsException
         *         if {@code length} is greater than {@code this.readableBytes}
         */
        public static int BytesBefore(this IByteBuf buf, int length, byte value)
        {
            return BytesBefore(buf, buf.ReaderIndex, buf.ReadableBytes(), value);
        }

        /**
         * Locates the first occurrence of the specified {@code value} in this
         * buffer.  The search starts from the specified {@code index} (inclusive)
         * and lasts for the specified {@code length}.
         * <p>
         * This method does not modify {@code readerIndex} or {@code writerIndex} of
         * this buffer.
         *
         * @return the number of bytes between the specified {@code index}
         *         and the first occurrence if found. {@code -1} otherwise.
         *
         * @throws IndexOutOfBoundsException
         *         if {@code index + length} is greater than {@code this.capacity}
         */
        public static int BytesBefore(this IByteBuf buf, int index, int length, byte value)
        {
            if (length < 0) throw new IndexOutOfRangeException();
            if (index + length > buf.WriterIndex) throw new IndexOutOfRangeException();
            int count = 0;
            bool found = false;
            buf.GetBytes(index, length, (b, start, len) =>
            {
                int i = Array.IndexOf(b, value, start, len);
                if (i >= 0)
                {
                    count += i;
                    found = true;
                    return i;
                }
                else
                {
                    count += len;
                    return len;
                }
            });
            return found ? count : -1;
        }
    }
}
