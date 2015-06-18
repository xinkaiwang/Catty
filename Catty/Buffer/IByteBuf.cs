using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Buffer
{
    // return int means how many data has been visited, most of times return = length. When return < length means end of visiting.
    public delegate int DataVisitor(byte[] buf, int index, int length);

    public interface IByteBuf
    {
        /**
         * Returns the number of bytes (octets) this buffer can contain.
         */
        int Capacity { get; }

        /**
         * Adjusts the capacity of this buffer.  If the {@code newCapacity} is less than the current
         * capacity, the content of this buffer is truncated.  If the {@code newCapacity} is greater
         * than the current capacity, the buffer is appended with unspecified data whose length is
         * {@code (newCapacity - currentCapacity)}.
         */
        IByteBuf ChangeCapacity(int newCapacity);

        /**
         * Returns the number of readable bytes which is equal to
         * {@code (this.writerIndex - this.readerIndex)}.
         */
        int ReadableBytes { get; }

        /**
         * Returns the {@code readerIndex} of this buffer.
         */
        int ReaderIndex { get; set; }


        /**
         * Returns the {@code writerIndex} of this buffer.
         */
        int WriterIndex { get; set; }

        /**
         * Sets the {@code readerIndex} and {@code writerIndex} of this buffer
         * in one shot.  This method is useful when you have to worry about the
         * invocation order of {@link #readerIndex(int)} and {@link #writerIndex(int)}
         * methods.  For example, the following code will fail:
         *
         * <pre>
         * // Create a buffer whose readerIndex, writerIndex and capacity are
         * // 0, 0 and 8 respectively.
         * {@link ByteBuf} buf = {@link Unpooled}.buffer(8);
         *
         * // IndexOutOfBoundsException is thrown because the specified
         * // readerIndex (2) cannot be greater than the current writerIndex (0).
         * buf.readerIndex(2);
         * buf.writerIndex(4);
         * </pre>
         *
         * The following code will also fail:
         *
         * <pre>
         * // Create a buffer whose readerIndex, writerIndex and capacity are
         * // 0, 8 and 8 respectively.
         * {@link ByteBuf} buf = {@link Unpooled}.wrappedBuffer(new byte[8]);
         *
         * // readerIndex becomes 8.
         * buf.readLong();
         *
         * // IndexOutOfBoundsException is thrown because the specified
         * // writerIndex (4) cannot be less than the current readerIndex (8).
         * buf.writerIndex(4);
         * buf.readerIndex(2);
         * </pre>
         *
         * By contrast, this method guarantees that it never
         * throws an {@link IndexOutOfBoundsException} as long as the specified
         * indexes meet basic constraints, regardless what the current index
         * values of the buffer are:
         *
         * <pre>
         * // No matter what the current state of the buffer is, the following
         * // call always succeeds as long as the capacity of the buffer is not
         * // less than 4.
         * buf.setIndex(2, 4);
         * </pre>
         *
         * @throws IndexOutOfBoundsException
         *         if the specified {@code readerIndex} is less than 0,
         *         if the specified {@code writerIndex} is less than the specified
         *         {@code readerIndex} or if the specified {@code writerIndex} is
         *         greater than {@code this.capacity}
         */
        IByteBuf SetIndex(int readerIndex, int writerIndex);


        /**
         * Sets the {@code readerIndex} and {@code writerIndex} of this buffer to
         * {@code 0}.
         * This method is identical to {@link #setIndex(int, int) setIndex(0, 0)}.
         * <p>
         * Please note that the behavior of this method is different
         * from that of NIO buffer, which sets the {@code limit} to
         * the {@code capacity} of the buffer.
         */
        IByteBuf Clear();

        /**
         * Marks the current {@code readerIndex} in this buffer.  You can
         * reposition the current {@code readerIndex} to the marked
         * {@code readerIndex} by calling {@link #resetReaderIndex()}.
         * The initial value of the marked {@code readerIndex} is {@code 0}.
         */
        IByteBuf MarkReaderIndex();

        /**
         * Repositions the current {@code readerIndex} to the marked
         * {@code readerIndex} in this buffer.
         *
         * @throws IndexOutOfBoundsException
         *         if the current {@code writerIndex} is less than the marked
         *         {@code readerIndex}
         */
        IByteBuf ResetReaderIndex();

        /**
         * Marks the current {@code writerIndex} in this buffer.  You can
         * reposition the current {@code writerIndex} to the marked
         * {@code writerIndex} by calling {@link #resetWriterIndex()}.
         * The initial value of the marked {@code writerIndex} is {@code 0}.
         */
        IByteBuf MarkWriterIndex();

        /**
         * Repositions the current {@code writerIndex} to the marked
         * {@code writerIndex} in this buffer.
         *
         * @throws IndexOutOfBoundsException
         *         if the current {@code readerIndex} is greater than the marked
         *         {@code writerIndex}
         */
        IByteBuf ResetWriterIndex();

        /**
         * Gets a byte at the specified absolute {@code index} in this buffer.
         * This method does not modify {@code readerIndex} or {@code writerIndex} of
         * this buffer.
         *
         * @throws IndexOutOfBoundsException
         *         if the specified {@code index} is less than {@code 0} or
         *         {@code index + 1} is greater than {@code this.capacity}
         */
        byte GetByte(int index);

        int GetBytes(int index, int length, DataVisitor visitor);

        /**
         * Sets the specified byte at the specified absolute {@code index} in this
         * buffer.  The 24 high-order bits of the specified value are ignored.
         * This method does not modify {@code readerIndex} or {@code writerIndex} of
         * this buffer.
         *
         * @throws IndexOutOfBoundsException
         *         if the specified {@code index} is less than {@code 0} or
         *         {@code index + 1} is greater than {@code this.capacity}
         */
        IByteBuf SetByte(int index, byte value);

        int SetBytes(int index, int length, DataVisitor visitor);

        /**
         * Gets a byte at the current {@code readerIndex} and increases
         * the {@code readerIndex} by {@code 1} in this buffer.
         *
         * @throws IndexOutOfBoundsException
         *         if {@code this.readableBytes} is less than {@code 1}
         */
        byte ReadByte();

        int ReadBytes(int length, DataVisitor visitor);

        /**
         * Sets the specified byte at the current {@code writerIndex}
         * and increases the {@code writerIndex} by {@code 1} in this buffer.
         * The 24 high-order bits of the specified value are ignored.
         *
         * @throws IndexOutOfBoundsException
         *         if {@code this.writableBytes} is less than {@code 1}
         */
        IByteBuf WriteByte(byte value);

        int WriteBytes(int length, DataVisitor visitor);

        IByteBuf Retain();
        void Release();

        IByteBuf Touch(Object hint);

        IByteBuf Compact();
    }
}
