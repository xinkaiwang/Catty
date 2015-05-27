using Catty.Core.Buffer;
using Catty.Core.Channel;
using Catty.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Handler.Codec
{
    public class StringEncoder : OneToOneEncoder
    {
        protected override object Encode(IChannelHandlerContext ctx, IChannel channel, object msg)
        {
            if (msg != null && msg is string)
            {
                IChannelBuffer buf = ChannelBuffers.DynamicBuffer(100);
                byte[] bytes = DataTypeString.BytesFromString((string)msg);
                buf.WriteBytes(bytes,0, bytes.Length);
                return buf;
            }
            else
            {
                return ChannelBuffers.EMPTY_BUFFER;
            }
        }
    }
}
