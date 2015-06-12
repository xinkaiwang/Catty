using Catty.Core.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Sockets
{
    public abstract class AbstractServerChannel : AbstractChannel, IServerChannel
    {
        public AbstractServerChannel(IChannelFactory factory, IChannelPipeline pipeline, IChannelSink sink)
            : base(null, factory, pipeline, sink)
        {
        }

        public override IChannelFuture Connect(EndPoint remoteAddress)
        {
            return GetUnsupportedOperationFuture();
        }

        public override IChannelFuture Disconnect()
        {
            return GetUnsupportedOperationFuture();
        }

        public override int GetInterestOps()
        {
            return ChannelValues.OP_NONE;
        }

        public override IChannelFuture SetInterestOps(int interestOps)
        {
            return GetUnsupportedOperationFuture();
        }

        protected override void SetInterestOpsNow(int interestOps)
        {
            // Ignore.
        }

        public override IChannelFuture Write(Object message)
        {
            return GetUnsupportedOperationFuture();
        }

        public override IChannelFuture Write(Object message, EndPoint remoteAddress)
        {
            return GetUnsupportedOperationFuture();
        }

        public override bool IsConnected()
        {
            return false;
        }

    }
}
