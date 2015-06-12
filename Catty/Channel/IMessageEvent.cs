using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Channel
{
    public interface IMessageEvent : IChannelEvent
    {
        Object GetMessage();

        EndPoint GetRemoteAddress();
    }
}
