using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Buffer
{
    public class BytesPool
    {
        public static BytesPool Instance = new BytesPool();
        public static BytesPool GetInstance() { return Instance; }

        public static void Release(ref byte[] bytes) 
        {
            GetInstance()._Release(bytes);
            bytes = null;
        }
        public static void Release(byte[] bytes)
        {
            GetInstance()._Release(bytes);
        }

        public static byte[] GetByteArray()
        {
            return GetInstance()._GetByteArray();
        }

        public const int PageSize = 1024; // 1k per buf
        private const int maxPoolSize = 1024 * 128; // 128M bytes
        private const int allocationBatchSize = 1024;
        private Stack<byte[]> pool = new Stack<byte[]>();
        private BytesPool()
        {
            GenerateMore();
        }

        private void GenerateMore()
        {
            for (int i = 0; i < allocationBatchSize; i++)
            {
                byte[] buf = new byte[PageSize];
                _Release(buf);
            }
        }

        private void _Release(byte[] bytes)
        {
            if (bytes != null && bytes.Length == PageSize)
            {
                lock (this)
                {
                    // when pool size reach limit, we simply abandon this object, let GC handle it.
                    if (pool.Count < maxPoolSize)
                    {
                        pool.Push(bytes);
                    }
                }
            }
        }

        private byte[] _GetByteArray()
        {
            byte[] bytes = null;
            lock (this)
            {
                if (pool.Count > 0)
                {
                    bytes = pool.Pop();
                }
            }
            if (bytes == null)
            {
                GenerateMore();
                bytes = _GetByteArray();
            }
            return bytes;
        }

    }
}
