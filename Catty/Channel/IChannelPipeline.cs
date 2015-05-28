using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Channel
{
    public interface IChannelPipeline
    {

        /**
         * Inserts a {@link ChannelHandler} at the first position of this pipeline.
         *
         * @param name     the name of the handler to insert first
         * @param handler  the handler to insert first
         *
         * @throws IllegalArgumentException
         *         if there's an entry with the same name already in the pipeline
         * @throws NullPointerException
         *         if the specified name or handler is {@code null}
         */
        void AddFirst(String name, IChannelHandler handler);

        /**
         * Appends a {@link ChannelHandler} at the last position of this pipeline.
         *
         * @param name     the name of the handler to append
         * @param handler  the handler to append
         *
         * @throws IllegalArgumentException
         *         if there's an entry with the same name already in the pipeline
         * @throws NullPointerException
         *         if the specified name or handler is {@code null}
         */
        void AddLast(String name, IChannelHandler handler);

        /**
         * Inserts a {@link ChannelHandler} before an existing handler of this
         * pipeline.
         *
         * @param baseName  the name of the existing handler
         * @param name      the name of the handler to insert before
         * @param handler   the handler to insert before
         *
         * @throws NoSuchElementException
         *         if there's no such entry with the specified {@code baseName}
         * @throws IllegalArgumentException
         *         if there's an entry with the same name already in the pipeline
         * @throws NullPointerException
         *         if the specified baseName, name, or handler is {@code null}
         */
        void AddBefore(String baseName, String name, IChannelHandler handler);

        /**
         * Inserts a {@link ChannelHandler} after an existing handler of this
         * pipeline.
         *
         * @param baseName  the name of the existing handler
         * @param name      the name of the handler to insert after
         * @param handler   the handler to insert after
         *
         * @throws NoSuchElementException
         *         if there's no such entry with the specified {@code baseName}
         * @throws IllegalArgumentException
         *         if there's an entry with the same name already in the pipeline
         * @throws NullPointerException
         *         if the specified baseName, name, or handler is {@code null}
         */
        void AddAfter(String baseName, String name, IChannelHandler handler);

        /**
         * Removes the specified {@link ChannelHandler} from this pipeline.
         *
         * @throws NoSuchElementException
         *         if there's no such handler in this pipeline
         * @throws NullPointerException
         *         if the specified handler is {@code null}
         */
        void Remove(IChannelHandler handler);

        /**
         * Removes the {@link ChannelHandler} with the specified name from this
         * pipeline.
         *
         * @return the removed handler
         *
         * @throws NoSuchElementException
         *         if there's no such handler with the specified name in this pipeline
         * @throws NullPointerException
         *         if the specified name is {@code null}
         */
        IChannelHandler Remove(String name);

        /**
         * Removes the {@link ChannelHandler} of the specified type from this
         * pipeline
         *
         * @param <T>          the type of the handler
         * @param handlerType  the type of the handler
         *
         * @return the removed handler
         *
         * @throws NoSuchElementException
         *         if there's no such handler of the specified type in this pipeline
         * @throws NullPointerException
         *         if the specified handler type is {@code null}
         */
        T Remove<T>() where T : IChannelHandler;

        /**
         * Removes the first {@link ChannelHandler} in this pipeline.
         *
         * @return the removed handler
         *
         * @throws NoSuchElementException
         *         if this pipeline is empty
         */
        IChannelHandler RemoveFirst();

        /**
         * Removes the last {@link ChannelHandler} in this pipeline.
         *
         * @return the removed handler
         *
         * @throws NoSuchElementException
         *         if this pipeline is empty
         */
        IChannelHandler RemoveLast();

        /**
         * Replaces the specified {@link ChannelHandler} with a new handler in
         * this pipeline.
         *
         * @throws NoSuchElementException
         *         if the specified old handler does not exist in this pipeline
         * @throws IllegalArgumentException
         *         if a handler with the specified new name already exists in this
         *         pipeline, except for the handler to be replaced
         * @throws NullPointerException
         *         if the specified old handler, new name, or new handler is
         *         {@code null}
         */
        void Replace(IChannelHandler oldHandler, String newName, IChannelHandler newHandler);

        /**
         * Replaces the {@link ChannelHandler} of the specified name with a new
         * handler in this pipeline.
         *
         * @return the removed handler
         *
         * @throws NoSuchElementException
         *         if the handler with the specified old name does not exist in this pipeline
         * @throws IllegalArgumentException
         *         if a handler with the specified new name already exists in this
         *         pipeline, except for the handler to be replaced
         * @throws NullPointerException
         *         if the specified old handler, new name, or new handler is
         *         {@code null}
         */
        IChannelHandler Replace(String oldName, String newName, IChannelHandler newHandler);

        /**
         * Replaces the {@link ChannelHandler} of the specified type with a new
         * handler in this pipeline.
         *
         * @return the removed handler
         *
         * @throws NoSuchElementException
         *         if the handler of the specified old handler type does not exist
         *         in this pipeline
         * @throws IllegalArgumentException
         *         if a handler with the specified new name already exists in this
         *         pipeline, except for the handler to be replaced
         * @throws NullPointerException
         *         if the specified old handler, new name, or new handler is
         *         {@code null}
         */
        T Replace<T>(String newName, IChannelHandler newHandler) where T : IChannelHandler;

        /**
         * Returns the first {@link ChannelHandler} in this pipeline.
         *
         * @return the first handler.  {@code null} if this pipeline is empty.
         */
        IChannelHandler GetFirst();

        /**
         * Returns the last {@link ChannelHandler} in this pipeline.
         *
         * @return the last handler.  {@code null} if this pipeline is empty.
         */
        IChannelHandler GetLast();

        /**
         * Returns the {@link ChannelHandler} with the specified name in this
         * pipeline.
         *
         * @return the handler with the specified name.
         *         {@code null} if there's no such handler in this pipeline.
         */
        IChannelHandler Get(String name);

        /**
         * Returns the {@link ChannelHandler} of the specified type in this
         * pipeline.
         *
         * @return the handler of the specified handler type.
         *         {@code null} if there's no such handler in this pipeline.
         */
        T Get<T>() where T : IChannelHandler;

        /**
         * Returns the context object of the specified {@link ChannelHandler} in
         * this pipeline.
         *
         * @return the context object of the specified handler.
         *         {@code null} if there's no such handler in this pipeline.
         */
        IChannelHandlerContext GetContext(IChannelHandler handler);

        /**
         * Returns the context object of the {@link ChannelHandler} with the
         * specified name in this pipeline.
         *
         * @return the context object of the handler with the specified name.
         *         {@code null} if there's no such handler in this pipeline.
         */
        IChannelHandlerContext GetContext(String name);

        /**
         * Returns the context object of the {@link ChannelHandler} of the
         * specified type in this pipeline.
         *
         * @return the context object of the handler of the specified type.
         *         {@code null} if there's no such handler in this pipeline.
         */
        IChannelHandlerContext GetContext<T>() where T : IChannelHandler;

        /**
         * Sends the specified {@link ChannelEvent} to the first
         * {@link ChannelUpstreamHandler} in this pipeline.
         *
         * @throws NullPointerException
         *         if the specified event is {@code null}
         */
        void SendUpstream(IChannelEvent e);

        /**
         * Sends the specified {@link ChannelEvent} to the last
         * {@link ChannelDownstreamHandler} in this pipeline.
         *
         * @throws NullPointerException
         *         if the specified event is {@code null}
         */
        void SendDownstream(IChannelEvent e);

        /**
         * Schedules the specified task to be executed in the I/O thread associated
         * with this pipeline's {@link Channel}.
         */
        IChannelFuture Execute(Action task);

        /**
         * Returns the {@link Channel} that this pipeline is attached to.
         *
         * @return the channel. {@code null} if this pipeline is not attached yet.
         */
        IChannel GetChannel();

        /**
         * Returns the {@link ChannelSink} that this pipeline is attached to.
         *
         * @return the sink. {@code null} if this pipeline is not attached yet.
         */
        IChannelSink GetSink();

        /**
         * Attaches this pipeline to the specified {@link Channel} and
         * {@link ChannelSink}.  Once a pipeline is attached, it can't be detached
         * nor attached again.
         *
         * @throws IllegalStateException if this pipeline is attached already
         */
        void Attach(IChannel channel, IChannelSink sink);

        /**
         * Returns {@code true} if and only if this pipeline is attached to
         * a {@link Channel}.
         */
        bool IsAttached();

        /**
         * Returns the {@link List} of the handler names.
         */
        List<String> GetNames();

        /**
         * Converts this pipeline into an ordered {@link Map} whose keys are
         * handler names and whose values are handlers.
         */
        IList<KeyValuePair<String, IChannelHandler>> ToMap();

    }
}
