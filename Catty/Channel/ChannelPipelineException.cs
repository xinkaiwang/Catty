using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catty.Core.Channel
{
    public class ChannelPipelineException : ChannelException
    {
        /**
         * Creates a new instance.
         */
        public ChannelPipelineException()
        {
        }

        /**
         * Creates a new instance.
         */
        public ChannelPipelineException(String message, Exception cause)
            : base(message, cause)
        {
        }

        /**
         * Creates a new instance.
         */
        public ChannelPipelineException(String message)
            : base(message)
        {
        }

    }
}
