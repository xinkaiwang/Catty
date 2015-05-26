using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catty.Core.Channel
{
    public interface IExceptionEvent : IChannelEvent
    {
        /**
         * Returns the raised exception.
         */
        Exception GetCause();
    }
}
