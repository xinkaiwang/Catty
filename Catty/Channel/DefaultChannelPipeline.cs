using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Catty.Core.Channel
{
    public class DefaultChannelPipeline : IChannelPipeline
    {
        static readonly IChannelSink discardingSink = new DiscardingChannelSink();

        private volatile IChannel channel;
        private volatile IChannelSink sink;

        private volatile DefaultChannelHandlerContext head;
        private volatile DefaultChannelHandlerContext tail;
        private readonly Dictionary<String, DefaultChannelHandlerContext> name2ctx = new Dictionary<String, DefaultChannelHandlerContext>(4);

        public IChannel GetChannel()
        {
            return channel;
        }

        public IChannelSink GetSink()
        {
            IChannelSink sink = this.sink;
            if (sink == null)
            {
                return discardingSink;
            }
            return sink;
        }

        public void Attach(IChannel channel, IChannelSink sink)
        {
            if (channel == null)
            {
                throw new NullReferenceException("channel");
            }
            if (sink == null)
            {
                throw new NullReferenceException("sink");
            }
            if (this.channel != null || this.sink != null)
            {
                throw new ArgumentException("attached already");
            }
            this.channel = channel;
            this.sink = sink;
        }

        public bool IsAttached()
        {
            return sink != null;
        }

        public void AddFirst(string name, IChannelHandler handler)
        {
            if (name2ctx.Count == 0)
            {
                Init(name, handler);
            }
            else
            {
                CheckDuplicateName(name);
                DefaultChannelHandlerContext oldHead = head;
                DefaultChannelHandlerContext newHead = new DefaultChannelHandlerContext(this, null, oldHead, name, handler);

                CallBeforeAdd(newHead);

                oldHead.prev = newHead;
                head = newHead;
                name2ctx.Add(name, newHead);

                CallAfterAdd(newHead);
            }
        }

        public void AddLast(string name, IChannelHandler handler)
        {
            if (name2ctx.Count == 0)
            {
                Init(name, handler);
            }
            else
            {
                CheckDuplicateName(name);
                DefaultChannelHandlerContext oldTail = tail;
                DefaultChannelHandlerContext newTail = new DefaultChannelHandlerContext(this, oldTail, null, name, handler);

                CallBeforeAdd(newTail);

                oldTail.next = newTail;
                tail = newTail;
                name2ctx.Add(name, newTail);

                CallAfterAdd(newTail);
            }
        }

        public void AddBefore(string baseName, string name, IChannelHandler handler)
        {
            DefaultChannelHandlerContext ctx = GetContextOrDie(baseName);
            if (ctx == head)
            {
                AddFirst(name, handler);
            }
            else
            {
                CheckDuplicateName(name);
                DefaultChannelHandlerContext newCtx = new DefaultChannelHandlerContext(this, ctx.prev, ctx, name, handler);

                CallBeforeAdd(newCtx);

                ctx.prev.next = newCtx;
                ctx.prev = newCtx;
                name2ctx.Add(name, newCtx);

                CallAfterAdd(newCtx);
            }
        }

        public void AddAfter(string baseName, string name, IChannelHandler handler)
        {
            DefaultChannelHandlerContext ctx = GetContextOrDie(baseName);
            if (ctx == tail)
            {
                AddLast(name, handler);
            }
            else
            {
                CheckDuplicateName(name);
                DefaultChannelHandlerContext newCtx = new DefaultChannelHandlerContext(this, ctx, ctx.next, name, handler);

                CallBeforeAdd(newCtx);

                ctx.next.prev = newCtx;
                ctx.next = newCtx;
                name2ctx.Add(name, newCtx);

                CallAfterAdd(newCtx);
            }
        }

        public void Remove(IChannelHandler handler)
        {
            Remove(GetContextOrDie(handler));
        }

        public IChannelHandler Remove(string name)
        {
            return Remove(GetContextOrDie(name)).GetHandler();
        }

        public T Remove<T>() where T : IChannelHandler
        {
            return (T)Remove(GetContextOrDie<T>()).GetHandler();
        }

        private DefaultChannelHandlerContext Remove(DefaultChannelHandlerContext ctx)
        {
            if (head == tail)
            {
                CallBeforeRemove(ctx);

                head = tail = null;
                name2ctx.Clear();

                CallAfterRemove(ctx);
            }
            else if (ctx == head)
            {
                RemoveFirst();
            }
            else if (ctx == tail)
            {
                RemoveLast();
            }
            else
            {
                CallBeforeRemove(ctx);

                DefaultChannelHandlerContext prev = ctx.prev;
                DefaultChannelHandlerContext next = ctx.next;
                prev.next = next;
                next.prev = prev;
                name2ctx.Remove(ctx.GetName());

                CallAfterRemove(ctx);
            }
            return ctx;
        }

        public IChannelHandler RemoveFirst()
        {
            if (name2ctx.Count == 0)
            {
                throw new IndexOutOfRangeException();
            }

            DefaultChannelHandlerContext oldHead = head;
            if (oldHead == null)
            {
                throw new IndexOutOfRangeException();
            }

            CallBeforeRemove(oldHead);

            if (oldHead.next == null)
            {
                head = tail = null;
                name2ctx.Clear();
            }
            else
            {
                oldHead.next.prev = null;
                head = oldHead.next;
                name2ctx.Remove(oldHead.GetName());
            }

            CallAfterRemove(oldHead);

            return oldHead.GetHandler();
        }

        public IChannelHandler RemoveLast()
        {
            if (name2ctx.Count == 0)
            {
                throw new IndexOutOfRangeException();
            }

            DefaultChannelHandlerContext oldTail = tail;
            if (oldTail == null)
            {
                throw new IndexOutOfRangeException();
            }

            CallBeforeRemove(oldTail);

            if (oldTail.prev == null)
            {
                head = tail = null;
                name2ctx.Clear();
            }
            else
            {
                oldTail.prev.next = null;
                tail = oldTail.prev;
                name2ctx.Remove(oldTail.GetName());
            }

            CallAfterRemove(oldTail);

            return oldTail.GetHandler();
        }

        public void Replace(IChannelHandler oldHandler, string newName, IChannelHandler newHandler)
        {
            throw new NotImplementedException();
        }

        public IChannelHandler Replace(string oldName, string newName, IChannelHandler newHandler)
        {
            throw new NotImplementedException();
        }

        public T Replace<T>(string newName, IChannelHandler newHandler) where T : IChannelHandler
        {
            throw new NotImplementedException();
        }

        private static void CallBeforeAdd(IChannelHandlerContext ctx)
        {
            if (!(ctx.GetHandler() is ILifeCycleAwareChannelHandler))
            {
                return;
            }

            ILifeCycleAwareChannelHandler h =
                (ILifeCycleAwareChannelHandler)ctx.GetHandler();

            try
            {
                h.BeforeAdd(ctx);
            }
            catch (Exception t)
            {
                throw new ChannelHandlerLifeCycleException(
                        h.GetType().Name +
                        ".beforeAdd() has thrown an exception; not adding.", t);
            }
        }

        private void CallAfterAdd(IChannelHandlerContext ctx)
        {
            if (!(ctx.GetHandler() is ILifeCycleAwareChannelHandler))
            {
                return;
            }

            ILifeCycleAwareChannelHandler h =
                (ILifeCycleAwareChannelHandler)ctx.GetHandler();

            try
            {
                h.AfterAdd(ctx);
            }
            catch (Exception t)
            {
                bool removed = false;
                try
                {
                    Remove((DefaultChannelHandlerContext)ctx);
                    removed = true;
                }
                catch (Exception t2)
                {
                    //if (logger.isWarnEnabled()) {
                    //    logger.warn("Failed to remove a handler: " + ctx.getName(), t2);
                    //}
                }

                if (removed)
                {
                    throw new ChannelHandlerLifeCycleException(
                            h.GetType().Name +
                            ".AfterAdd() has thrown an exception; removed.", t);
                }
                else
                {
                    throw new ChannelHandlerLifeCycleException(
                            h.GetType().Name +
                            ".afterAdd() has thrown an exception; also failed to remove.", t);
                }
            }
        }

        private static void CallBeforeRemove(IChannelHandlerContext ctx)
        {
            if (!(ctx.GetHandler() is ILifeCycleAwareChannelHandler))
            {
                return;
            }

            ILifeCycleAwareChannelHandler h =
                (ILifeCycleAwareChannelHandler)ctx.GetHandler();

            try
            {
                h.BeforeRemove(ctx);
            }
            catch (Exception t)
            {
                throw new ChannelHandlerLifeCycleException(
                        h.GetType().Name +
                        ".BeforeRemove() has thrown an exception; not removing.", t);
            }
        }

        private static void CallAfterRemove(IChannelHandlerContext ctx)
        {
            if (!(ctx.GetHandler() is ILifeCycleAwareChannelHandler))
            {
                return;
            }

            ILifeCycleAwareChannelHandler h =
                (ILifeCycleAwareChannelHandler)ctx.GetHandler();

            try
            {
                h.AfterRemove(ctx);
            }
            catch (Exception t)
            {
                throw new ChannelHandlerLifeCycleException(
                        h.GetType().Name +
                        ".afterRemove() has thrown an exception.", t);
            }
        }

        public IChannelHandler GetFirst()
        {
            DefaultChannelHandlerContext head = this.head;
            if (head == null)
            {
                return null;
            }
            return head.GetHandler();
        }

        public IChannelHandler GetLast()
        {
            DefaultChannelHandlerContext tail = this.tail;
            if (tail == null)
            {
                return null;
            }
            return tail.GetHandler();
        }

        public IChannelHandler Get(String name)
        {
            if (name2ctx.ContainsKey(name))
            {
                DefaultChannelHandlerContext ctx = name2ctx[name];
                return ctx.GetHandler();
            }
            else
            {
                return null;
            }
        }

        public T Get<T>() where T : IChannelHandler
        {
            IChannelHandlerContext ctx = GetContext<T>();
            if (ctx == null)
            {
                return default(T);
            }
            else
            {
                T handler = (T)ctx.GetHandler();
                return handler;
            }
        }

        public IChannelHandlerContext GetContext(string name)
        {
            if (name == null)
                return null;
            return name2ctx[name];
        }

        public IChannelHandlerContext GetContext(IChannelHandler handler)
        {
            if (handler == null)
                return null;
            return name2ctx.Values.SingleOrDefault(o => o.GetHandler() == handler);
        }

        public IChannelHandlerContext GetContext<T>() where T : IChannelHandler
        {
            return name2ctx.Values.SingleOrDefault(o => o is T);
        }

        public void SendUpstream(IChannelEvent e)
        {
            DefaultChannelHandlerContext head = GetActualUpstreamContext(this.head);
            if (head == null)
            {
                //if (logger.isWarnEnabled())
                //{
                //    logger.warn(
                //            "The pipeline contains no upstream handlers; discarding: " + e);
                //}

                return;
            }

            SendUpstream(head, e);
        }

        internal void SendUpstream(DefaultChannelHandlerContext ctx, IChannelEvent e)
        {
            try
            {
                ((IChannelUpstreamHandler)ctx.GetHandler()).HandleUpstream(ctx, e);
            }
            catch (Exception t)
            {
                NotifyHandlerException(e, t);
            }
        }

        public void SendDownstream(IChannelEvent e)
        {
            DefaultChannelHandlerContext tail = GetActualDownstreamContext(this.tail);
            if (tail == null)
            {
                try
                {
                    GetSink().EventSunk(this, e);
                    return;
                }
                catch (Exception t)
                {
                    NotifyHandlerException(e, t);
                    return;
                }
            }

            SendDownstream(tail, e);
        }

        internal void SendDownstream(DefaultChannelHandlerContext ctx, IChannelEvent e)
        {
            if (e is UpstreamMessageEvent)
            {
                throw new ArgumentException("cannot send an upstream event to downstream");
            }

            try
            {
                ((IChannelDownstreamHandler)ctx.GetHandler()).HandleDownstream(ctx, e);
            }
            catch (Exception t)
            {
                // Unlike an upstream event, a downstream event usually has an
                // incomplete future which is supposed to be updated by ChannelSink.
                // However, if an exception is raised before the event reaches at
                // ChannelSink, the future is not going to be updated, so we update
                // here.
                e.GetFuture().SetFailure(t);
                NotifyHandlerException(e, t);
            }
        }

        private DefaultChannelHandlerContext GetActualUpstreamContext(DefaultChannelHandlerContext ctx)
        {
            if (ctx == null)
            {
                return null;
            }

            DefaultChannelHandlerContext realCtx = ctx;
            while (!realCtx.CanHandleUpstream())
            {
                realCtx = realCtx.next;
                if (realCtx == null)
                {
                    return null;
                }
            }

            return realCtx;
        }

        private DefaultChannelHandlerContext GetActualDownstreamContext(DefaultChannelHandlerContext ctx)
        {
            if (ctx == null)
            {
                return null;
            }

            DefaultChannelHandlerContext realCtx = ctx;
            while (!realCtx.CanHandleDownstream())
            {
                realCtx = realCtx.prev;
                if (realCtx == null)
                {
                    return null;
                }
            }

            return realCtx;
        }

        public IChannelFuture Execute(Action task)
        {
            throw new NotImplementedException();
        }

        internal void NotifyHandlerException(IChannelEvent e, Exception t)
        {
            if (e is IExceptionEvent)
            {
                //if (logger.isWarnEnabled()) {
                //    logger.warn(
                //            "An exception was thrown by a user handler " +
                //            "while handling an exception event (" + e + ')', t);
                //}

                return;
            }

            ChannelPipelineException pe;
            if (t is ChannelPipelineException)
            {
                pe = (ChannelPipelineException)t;
            }
            else
            {
                pe = new ChannelPipelineException("", t);
            }

            try
            {
                sink.ExceptionCaught(this, e, pe);
            }
            catch (Exception e1)
            {
                //if (logger.isWarnEnabled()) {
                //    logger.warn("An exception was thrown by an exception handler.", e1);
                //}
            }
        }

        private void Init(String name, IChannelHandler handler)
        {
            DefaultChannelHandlerContext ctx = new DefaultChannelHandlerContext(this, null, null, name, handler);
            CallBeforeAdd(ctx);
            head = tail = ctx;
            name2ctx.Clear();
            name2ctx.Add(name, ctx);
            CallAfterAdd(ctx);
        }

        private void CheckDuplicateName(String name)
        {
            if (name2ctx.ContainsKey(name))
            {
                throw new ArgumentException("Duplicate handler name: " + name);
            }
        }

        private DefaultChannelHandlerContext GetContextOrDie(String name)
        {
            DefaultChannelHandlerContext ctx = (DefaultChannelHandlerContext)GetContext(name);
            if (ctx == null)
            {
                throw new KeyNotFoundException(name);
            }
            else
            {
                return ctx;
            }
        }

        private DefaultChannelHandlerContext GetContextOrDie(IChannelHandler handler)
        {
            DefaultChannelHandlerContext ctx = (DefaultChannelHandlerContext)GetContext(handler);
            if (ctx == null)
            {
                throw new KeyNotFoundException(handler.GetType().Name);
            }
            else
            {
                return ctx;
            }
        }

        private DefaultChannelHandlerContext GetContextOrDie<T>() where T : IChannelHandler
        {
            DefaultChannelHandlerContext ctx = (DefaultChannelHandlerContext)GetContext<T>();
            if (ctx == null)
            {
                throw new KeyNotFoundException(typeof(T).Name);
            }
            else
            {
                return ctx;
            }
        }

        public List<string> GetNames()
        {
            List<string> list = new List<string>();
            var item = this.head;
            while (item != null)
            {
                list.Add(item.GetName());
                item = item.next;
            }
            return list;
        }

        public IList<KeyValuePair<String, IChannelHandler>> ToMap()
        {
            var names = GetNames();
            return names.Select(o => new KeyValuePair<String, IChannelHandler>(o, this.name2ctx[o].GetHandler())).ToList();
        }

        internal sealed class DefaultChannelHandlerContext : IChannelHandlerContext
        {
            private readonly DefaultChannelPipeline parent;
            internal volatile DefaultChannelHandlerContext next;
            internal volatile DefaultChannelHandlerContext prev;
            private readonly String name;
            private readonly IChannelHandler handler;
            private readonly bool canHandleUpstream;
            private readonly bool canHandleDownstream;
            private volatile Object attachment;

            internal DefaultChannelHandlerContext(DefaultChannelPipeline parent,
                    DefaultChannelHandlerContext prev, DefaultChannelHandlerContext next,
                    String name, IChannelHandler handler)
            {

                this.parent = parent;
                if (name == null)
                {
                    throw new NullReferenceException("name");
                }
                if (handler == null)
                {
                    throw new NullReferenceException("handler");
                }
                canHandleUpstream = handler is IChannelUpstreamHandler;
                canHandleDownstream = handler is IChannelDownstreamHandler;

                if (!canHandleUpstream && !canHandleDownstream)
                {
                    throw new ArgumentException(
                            "handler must be either " +
                            typeof(IChannelUpstreamHandler).Name + " or " +
                            typeof(IChannelDownstreamHandler).Name + '.');
                }

                this.prev = prev;
                this.next = next;
                this.name = name;
                this.handler = handler;
            }

            public IChannel GetChannel()
            {
                return GetPipeline().GetChannel();
            }

            public IChannelPipeline GetPipeline()
            {
                return parent;
            }

            public bool CanHandleDownstream()
            {
                return canHandleDownstream;
            }

            public bool CanHandleUpstream()
            {
                return canHandleUpstream;
            }

            public IChannelHandler GetHandler()
            {
                return handler;
            }

            public String GetName()
            {
                return name;
            }

            public Object GetAttachment()
            {
                return attachment;
            }

            public void SetAttachment(Object attachment)
            {
                this.attachment = attachment;
            }

            public void SendDownstream(IChannelEvent e)
            {
                DefaultChannelHandlerContext prev = parent.GetActualDownstreamContext(this.prev);
                if (prev == null)
                {
                    try
                    {
                        parent.GetSink().EventSunk(parent, e);
                    }
                    catch (Exception t)
                    {
                        parent.NotifyHandlerException(e, t);
                    }
                }
                else
                {
                    parent.SendDownstream(prev, e);
                }
            }

            public void SendUpstream(IChannelEvent e)
            {
                DefaultChannelHandlerContext next = parent.GetActualUpstreamContext(this.next);
                if (next != null)
                {
                    parent.SendUpstream(next, e);
                }
            }
        }

        private sealed class DiscardingChannelSink : IChannelSink
        {
            internal DiscardingChannelSink()
            {
            }

            public void EventSunk(IChannelPipeline pipeline, IChannelEvent e)
            {
                //if (logger.isWarnEnabled()) {
                //    logger.warn("Not attached yet; discarding: " + e);
                //}
            }

            public void ExceptionCaught(IChannelPipeline pipeline,
                    IChannelEvent e, ChannelPipelineException cause)
            {
                throw cause;
            }

            public IChannelFuture Execute(IChannelPipeline pipeline, Action task)
            {
                //if (logger.isWarnEnabled()) {
                //    logger.warn("Not attached yet; rejecting: " + task);
                //}
                return Channels.FailedFuture(pipeline.GetChannel(), new Exception("Not attached yet"));
            }
        }
    }

}
