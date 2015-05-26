using Catty.Core.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Sockets
{
    public interface ISocketChannelConfig : IChannelConfig
    {
        ///**
        // * Gets the {@link StandardSocketOptions#TCP_NODELAY} option.
        // */
        //bool IsTcpNoDelay();

        ///**
        // * Sets the {@link StandardSocketOptions#TCP_NODELAY} option.
        // */
        //void SetTcpNoDelay(bool tcpNoDelay);

        ///**
        // * Gets the {@link StandardSocketOptions#SO_LINGER} option.
        // */
        //int getSoLinger();

        ///**
        // * Sets the {@link StandardSocketOptions#SO_LINGER} option.
        // */
        //void setSoLinger(int soLinger);

        ///**
        // * Gets the {@link StandardSocketOptions#SO_SNDBUF} option.
        // */
        //int GetSendBufferSize();

        ///**
        // * Sets the {@link StandardSocketOptions#SO_SNDBUF} option.
        // */
        //void SetSendBufferSize(int sendBufferSize);

        ///**
        // * Gets the {@link StandardSocketOptions#SO_RCVBUF} option.
        // */
        //int GetReceiveBufferSize();

        ///**
        // * Sets the {@link StandardSocketOptions#SO_RCVBUF} option.
        // */
        //void SetReceiveBufferSize(int receiveBufferSize);

        ///**
        // * Gets the {@link StandardSocketOptions#SO_KEEPALIVE} option.
        // */
        //bool IsKeepAlive();

        ///**
        // * Sets the {@link StandardSocketOptions#SO_KEEPALIVE} option.
        // */
        //void SetKeepAlive(bool keepAlive);

        ///**
        // * Gets the {@link StandardSocketOptions#IP_TOS} option.
        // */
        //int GetTrafficClass();

        ///**
        // * Sets the {@link StandardSocketOptions#IP_TOS} option.
        // */
        //void SetTrafficClass(int trafficClass);

        ///**
        // * Gets the {@link StandardSocketOptions#SO_REUSEADDR} option.
        // */
        //bool IsReuseAddress();

        ///**
        // * Sets the {@link StandardSocketOptions#SO_REUSEADDR} option.
        // */
        //void SetReuseAddress(bool reuseAddress);

        ///**
        // * Sets the performance preferences as specified in
        // * {@link Socket#setPerformancePreferences(int, int, int)}.
        // */
        //void SetPerformancePreferences(
        //        int connectionTime, int latency, int bandwidth);
    }
}
