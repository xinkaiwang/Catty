﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Buffer
{
    public interface IChannelBufferFactory
    {
        IChannelBuffer GetBuffer(int capacity);

    }
}
