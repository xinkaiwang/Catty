using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catty.Core.Channel
{
    public class ChannelHandlerLifeCycleException : Exception
    {
        /**
         * Creates a new exception.
         */
        public ChannelHandlerLifeCycleException()
        {
        }

        /**
         * Creates a new exception.
         */
        public ChannelHandlerLifeCycleException(String message, Exception cause)
            :base(message, cause)
        {
        }

        /**
         * Creates a new exception.
         */
        public ChannelHandlerLifeCycleException(String message)
            :base(message)
        {
        }
    }
}
