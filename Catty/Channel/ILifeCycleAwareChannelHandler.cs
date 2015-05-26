using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catty.Core.Channel
{
    public interface ILifeCycleAwareChannelHandler : IChannelHandler
    {
        void BeforeAdd(IChannelHandlerContext ctx);
        void AfterAdd(IChannelHandlerContext ctx);
        void BeforeRemove(IChannelHandlerContext ctx);
        void AfterRemove(IChannelHandlerContext ctx);
    }
}
