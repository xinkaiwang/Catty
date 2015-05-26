using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Channel
{
    public enum ChannelState
    {
        OPEN,
        BOUND,
        CONNECTED,
        INTEREST_OPS,
    }
}
