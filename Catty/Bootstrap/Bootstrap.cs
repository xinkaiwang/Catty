using Catty.Core.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Bootstrap
{
    public class Bootstrap
    {
        private volatile IChannelFactory factory;
        private volatile IChannelPipelineFactory pipelineFactory;
        private volatile Dictionary<String, Object> options = new Dictionary<String, Object>();

        /**
         * Creates a new instance with no {@link ChannelFactory} set.
         * {@link #setFactory(ChannelFactory)} must be called at once before any
         * I/O operation is requested.
         */
        protected Bootstrap()
        {
        }

        /**
         * Creates a new instance with the specified initial {@link ChannelFactory}.
         */
        protected Bootstrap(IChannelFactory channelFactory)
        {
            SetFactory(channelFactory);
        }

        /**
         * Returns the {@link ChannelFactory} that will be used to perform an
         * I/O operation.
         *
         * @throws IllegalStateException
         *         if the factory is not set for this bootstrap yet.
         *         The factory can be set in the constructor or
         *         {@link #setFactory(ChannelFactory)}.
         */
        public IChannelFactory GetFactory()
        {
            IChannelFactory factory = this.factory;
            if (factory == null)
            {
                throw new InvalidOperationException("factory is not set yet.");
            }
            return factory;
        }

        /**
         * Sets the {@link ChannelFactory} that will be used to perform an I/O
         * operation.  This method can be called only once and can't be called at
         * all if the factory was specified in the constructor.
         *
         * @throws IllegalStateException
         *         if the factory is already set
         */
        public void SetFactory(IChannelFactory factory)
        {
            if (factory == null)
            {
                throw new NullReferenceException("factory");
            }
            if (this.factory != null)
            {
                throw new InvalidOperationException(
                        "factory can't change once set.");
            }
            this.factory = factory;
        }

        /**
         * Returns the {@link ChannelPipelineFactory} which creates a new
         * {@link ChannelPipeline} for each new {@link Channel}.
         *
         * @see #getPipeline()
         */
        public IChannelPipelineFactory GetPipelineFactory()
        {
            return pipelineFactory;
        }

        private class MyChannelPipelineFactory : IChannelPipelineFactory
        {
            Func<IChannelHandler[]> handlersFactory;
            public MyChannelPipelineFactory(Func<IChannelHandler[]> handlersFactory)
            {
                this.handlersFactory = handlersFactory;
            }
            public IChannelPipeline GetPipeline()
            {
                return Channels.Pipeline(handlersFactory());
            }
        }

        public void SetPipelineFactory(Func<IChannelHandler[]> handlers)
        {
            if (handlers == null)
            {
                throw new NullReferenceException("handlers");
            }
            var factory = new MyChannelPipelineFactory(handlers);
            this.pipelineFactory = factory;
        }

        /**
         * Sets the {@link ChannelPipelineFactory} which creates a new
         * {@link ChannelPipeline} for each new {@link Channel}.  Calling this
         * method invalidates the current {@code pipeline} property of this
         * bootstrap.  Subsequent {@link #getPipeline()} and {@link #getPipelineAsMap()}
         * calls will raise {@link IllegalStateException}.
         *
         * @see #setPipeline(ChannelPipeline)
         * @see #setPipelineAsMap(Map)
         */
        public void SetPipelineFactory(IChannelPipelineFactory pipelineFactory)
        {
            if (pipelineFactory == null)
            {
                throw new NullReferenceException("pipelineFactory");
            }
            this.pipelineFactory = pipelineFactory;
        }

        /**
         * Returns the options which configures a new {@link Channel} and its
         * child {@link Channel}s.  The names of the child {@link Channel} options
         * are prepended with {@code "child."} (e.g. {@code "child.keepAlive"}).
         */
        public Dictionary<String, Object> GetOptions()
        {
            return new Dictionary<String, Object>(options);
        }

        /**
         * Sets the options which configures a new {@link Channel} and its child
         * {@link Channel}s.  To set the options of a child {@link Channel}, prepend
         * {@code "child."} to the option name (e.g. {@code "child.keepAlive"}).
         */
        public void SetOptions(Dictionary<String, Object> options)
        {
            if (options == null)
            {
                throw new NullReferenceException("options");
            }
            this.options = new Dictionary<String, Object>(options);
        }

        /**
         * Returns the value of the option with the specified key.  To retrieve
         * the option value of a child {@link Channel}, prepend {@code "child."}
         * to the option name (e.g. {@code "child.keepAlive"}).
         *
         * @param key  the option name
         *
         * @return the option value if the option is found.
         *         {@code null} otherwise.
         */
        public Object GetOption(String key)
        {
            if (key == null)
            {
                throw new NullReferenceException("key");
            }
            if (options.ContainsKey(key))
            {
                return options[key];
            }
            else
            {
                return null;
            }
        }

        /**
         * Sets an option with the specified key and value.  If there's already
         * an option with the same key, it is replaced with the new value.  If the
         * specified value is {@code null}, an existing option with the specified
         * key is removed.  To set the option value of a child {@link Channel},
         * prepend {@code "child."} to the option name (e.g. {@code "child.keepAlive"}).
         *
         * @param key    the option name
         * @param value  the option value
         */
        public void SetOption(String key, Object value)
        {
            if (key == null)
            {
                throw new NullReferenceException("key");
            }
            if (value == null)
            {
                options.Remove(key);
            }
            else
            {
                options[key] = value;
            }
        }
    }
}
