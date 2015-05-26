using Catty.Core.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Sockets
{
    public interface IServerSocketChannelConfig : IChannelConfig
    {
        ///**
        // * Gets the backlog value to specify when the channel binds to a local
        // * address.
        // */
        //int GetBacklog();

        ///**
        // * Sets the backlog value to specify when the channel binds to a local
        // * address.
        // */
        //void SetBacklog(int backlog);

        ///**
        // * Gets the {@link StandardSocketOptions#SO_REUSEADDR} option.
        // */
        //bool isReuseAddress();

        ///**
        // * Sets the {@link StandardSocketOptions#SO_REUSEADDR} option.
        // */
        //void setReuseAddress(bool reuseAddress);

        ///**
        // * Sets the performance preferences as specified in
        // * {@link ServerSocket#setPerformancePreferences(int, int, int)}.
        // */
        //void setPerformancePreferences(int connectionTime, int latency, int bandwidth);
    }
}
