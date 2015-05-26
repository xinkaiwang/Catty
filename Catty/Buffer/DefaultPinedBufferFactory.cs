using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Buffer
{
    public class DefaultPinedBufferFactory : IPinedBufferFactory
    {
        private static DefaultPinedBufferFactory instance = new DefaultPinedBufferFactory();
        public static IPinedBufferFactory GetInstance()
        {
            return instance;
        }

        private object lockObj = new object(); // always lock before modify the list
        private List<byte[]> list = new List<byte[]>();
        private const int BUF_SIZE = 512;
        private const int BATCH_ALLOC_COUNT = 64;
        private int allocCounter = 0;

        public byte[] GetBuffer(int capacity)
        {
            byte[] buf = null;
            lock (this.lockObj)
            {
                if (list.Count == 0)
                {
                    for (int i = 0; i < BATCH_ALLOC_COUNT; i++)
                    {
                        allocCounter++;
                        list.Add(new byte[BUF_SIZE]);
                    }
                }
                buf = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
                return buf;
            }
        }

        public void ReleaseBuffer(byte[] buffer)
        {
            lock (this.lockObj)
            {
                list.Add(buffer);
            }
        }
    }
}
