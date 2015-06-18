using Catty.Core.Buffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Channel
{
    public interface IChannelConfig
    {
        /**
         * Sets the configuration properties from the specified {@link Map}.
         */
        void SetOptions(Dictionary<String, Object> options);

        /**
         * Sets a configuration property with the specified name and value.
         * To override this method properly, you must call the super class:
         * <pre>
         * public boolean setOption(String name, Object value) {
         *     if (super.setOption(name, value)) {
         *         return true;
         *     }
         *
         *     if (name.equals("additionalOption")) {
         *         ....
         *         return true;
         *     }
         *
         *     return false;
         * }
         * </pre>
         *
         * @return {@code true} if and only if the property has been set
         */
        bool SetOption(String name, Object value);

        /**
         * Returns the {@link ChannelPipelineFactory} which will be used when
         * a child channel is created.  If the {@link Channel} does not create
         * a child channel, this property is not used at all, and therefore will
         * be ignored.
         */
        IChannelPipelineFactory GetPipelineFactory();

        /**
         * Sets the {@link ChannelPipelineFactory} which will be used when
         * a child channel is created.  If the {@link Channel} does not create
         * a child channel, this property is not used at all, and therefore will
         * be ignored.
         */
        void SetPipelineFactory(IChannelPipelineFactory pipelineFactory);

        /**
         * Returns the connect timeout of the channel in milliseconds.  If the
         * {@link Channel} does not support connect operation, this property is not
         * used at all, and therefore will be ignored.
         *
         * @return the connect timeout in milliseconds.  {@code 0} if disabled.
         */
        int GetConnectTimeoutMillis();

        /**
         * Sets the connect timeout of the channel in milliseconds.  If the
         * {@link Channel} does not support connect operation, this property is not
         * used at all, and therefore will be ignored.
         *
         * @param connectTimeoutMillis the connect timeout in milliseconds.
         *                             {@code 0} to disable.
         */
        void SetConnectTimeoutMillis(int connectTimeoutMillis);
    }
}
