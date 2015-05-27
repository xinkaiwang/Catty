using Catty.Core.Channel;
using Catty.Core.Handler.Codec;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleServer.Handler
{
    public class JsonEncoder : OneToOneEncoder
    {
        protected override object Encode(IChannelHandlerContext ctx, IChannel channel, object msg)
        {
            return JsonConvert.SerializeObject(msg);
        }
    }
}
