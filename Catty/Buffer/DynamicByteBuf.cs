using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Buffer
{
    public class DynamicByteBuf : IByteBuf
    {
        public const int PAGESIZE = 4096;
        private int MacCapacity = int.MaxValue;

        private List<byte[]> pages = new List<byte[]>() { BytesPool.GetByteArray() };

        public int Capacity
        {
            get { return MacCapacity; }
        }

        public IByteBuf ChangeCapacity(int newCapacity)
        {
            if (this.WriterIndex > newCapacity) throw new IndexOutOfRangeException();
            this.MacCapacity = newCapacity;
            return this;
        }

        public int ReaderIndex { get; set; }
        public int WriterIndex { get; set; }

        public IByteBuf SetIndex(int readerIndex, int writerIndex)
        {
            if (readerIndex > writerIndex) throw new IndexOutOfRangeException();
            if (writerIndex > Capacity) throw new IndexOutOfRangeException();
            this.ReaderIndex = readerIndex;
            this.WriterIndex = writerIndex;
            return this;
        }

        public IByteBuf Clear()
        {
            this.ReaderIndex = 0;
            this.WriterIndex = 0;
            this.hint = null;
            if (pages.Count > 1)
            {
                for (int i = 1; i < pages.Count; i++)
                {
                    BytesPool.Release(pages[i]);
                }
                pages.RemoveRange(1, pages.Count - 1);
            }
            return this;
        }

        public int ReaderIndexMark;
        public int WriterIndexMark;
        public IByteBuf MarkReaderIndex()
        {
            this.ReaderIndexMark = this.ReaderIndex;
            return this;
        }

        public IByteBuf ResetReaderIndex()
        {
            this.ReaderIndex = this.ReaderIndexMark;
            return this;
        }

        public IByteBuf MarkWriterIndex()
        {
            this.WriterIndexMark = this.WriterIndex;
            return this;
        }

        public IByteBuf ResetWriterIndex()
        {
            this.WriterIndex = this.WriterIndexMark;
            return this;
        }

        public byte GetByte(int index)
        {
            int pageNum = index / PAGESIZE;
            if (pageNum > pages.Count) throw new IndexOutOfRangeException();
            return pages[pageNum][index % PAGESIZE];
        }

        public int GetBytes(int index, int length, DataVisitor visitor)
        {
            if (index + length > WriterIndex) throw new IndexOutOfRangeException();
            int count = 0;
            while(count < length) {
                int pageNum = index / PAGESIZE;
                int indexInPage = index % PAGESIZE;
                int len = Math.Min(PAGESIZE - indexInPage, length - count);
                int ret = visitor(pages[pageNum], indexInPage, len);
                index += ret;
                count += ret;
                if (ret < len) return count;
            }
            return count;
        }

        public IByteBuf SetByte(int index, byte value)
        {
            int pageNum = index / BytesPool.PageSize;
            if (pageNum > pages.Count) throw new IndexOutOfRangeException();
            while (pages.Count <= pageNum)
            {
                pages.Add(BytesPool.GetByteArray());
            }
            pages[pageNum][index % BytesPool.PageSize] = value;
            return this;
        }

        public int SetBytes(int index, int length, DataVisitor visitor)
        {
            if (index + length > WriterIndex) throw new IndexOutOfRangeException();
            int count = 0;
            while (count < length)
            {
                int pageNum = index / PAGESIZE;
                int indexInPage = index % PAGESIZE;
                while (pages.Count <= pageNum)
                {
                    pages.Add(BytesPool.GetByteArray());
                }
                int len = Math.Min(PAGESIZE - indexInPage, length - count);
                int ret = visitor(pages[pageNum], indexInPage, len);
                index += ret;
                count += ret;
                if (ret < len) return count;
            }
            return count;
        }

        public byte ReadByte()
        {
            int pageNum = ReaderIndex / BytesPool.PageSize;
            if (pageNum > pages.Count) throw new IndexOutOfRangeException();
            return pages[pageNum][(ReaderIndex++) % BytesPool.PageSize];
        }

        public int ReadBytes(int length, DataVisitor visitor)
        {
            if (ReaderIndex + length > WriterIndex) throw new IndexOutOfRangeException();
            int count = 0;
            while (count < length)
            {
                int pageNum = ReaderIndex / PAGESIZE;
                int indexInPage = ReaderIndex % PAGESIZE;
                int len = Math.Min(PAGESIZE - indexInPage, length - count);
                int ret = visitor(pages[pageNum], indexInPage, len);
                ReaderIndex += ret;
                count += ret;
                if (ret < len) return count;
            }
            return count;
        }

        public IByteBuf WriteByte(byte value)
        {
            int pageNum = WriterIndex / BytesPool.PageSize;
            if (pageNum > pages.Count) throw new IndexOutOfRangeException();
            pages[pageNum][(WriterIndex++) % BytesPool.PageSize] = value;
            return this;
        }

        public int WriteBytes(int length, DataVisitor visitor)
        {
            if (WriterIndex + length > Capacity) throw new IndexOutOfRangeException();
            int count = 0;
            while (count < length)
            {
                int pageNum = WriterIndex / PAGESIZE;
                int indexInPage = WriterIndex % PAGESIZE;
                int len = Math.Min(PAGESIZE - indexInPage, length - count);
                while (pages.Count <= pageNum)
                {
                    pages.Add(BytesPool.GetByteArray());
                }
                int ret = visitor(pages[pageNum], indexInPage, len);
                WriterIndex += ret;
                count += ret;
                if (ret < len) return count;
            }
            return count;
        }

        private int refCount = 1;
        public IByteBuf Retain()
        {
            refCount++;
            return this;
        }

        public void Release()
        {
            refCount--;
            if (refCount < 0) throw new IndexOutOfRangeException();
            if (refCount == 0)
            {
                this.Clear();
                RecycleObjectPool<DynamicByteBuf>.ReleaseObject(this);
            }
        }

        private object hint = null;
        public IByteBuf Touch(object hint)
        {
            this.hint = hint;
            return this;
        }

        public IByteBuf Compact()
        {
            if (this.ReadableBytes() == 0)
            {
                this.Clear();
            }
            else
            {
                int howManyPageToRemove = ReaderIndex / PAGESIZE;
                if (howManyPageToRemove > 0)
                {
                    ReaderIndex -= howManyPageToRemove * PAGESIZE;
                    ReaderIndexMark = ReaderIndex;
                    WriterIndex -= howManyPageToRemove * PAGESIZE;
                    WriterIndexMark -= WriterIndex;
                    for (int i = 0; i < howManyPageToRemove; i++)
                    {
                        BytesPool.Release(pages[i]);
                    }
                    pages.RemoveRange(0, howManyPageToRemove);
                }
            }
            return this;
        }
    }
}
