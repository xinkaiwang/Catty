using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Buffer
{
    // when we call socket.BeginSend() or BeginReceive(), the buffer is pined and not GC is not be able to move it.
    // You should always use PinedBufferFactory when you need a pined buffer. PinedBufferFactory typically allocate buffers in batch, to avoid fragment.
    public interface IPinedBufferFactory
    {
        byte[] GetBuffer(int capacity); // Returned buffer may come with different size from what you expected, you should not make any assumption on the buffer size.
        void ReleaseBuffer(byte[] buffer); // You should alwys relase the buffer after use.
    }
}
