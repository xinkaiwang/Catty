using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catty.Core.Channel
{
    public class ChannelException : Exception
    {
        /**
         * Creates a new exception.
         */
        public ChannelException()
        {
        }

        /**
         * Creates a new exception.
         */
        public ChannelException(String message, Exception cause)
            : base(message, cause)
        {
        }

        /**
         * Creates a new exception.
         */
        public ChannelException(String message)
            : base(message)
        {
        }

    }
}
