using Catty.Core.Buffer;
using Catty.Core.Channel;
using Catty.Core.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catty.Core.Handler.Codec
{
    public class FastLineBreakDecoderContext : IDisposable
    {
        public DynamicByteBuf inputBuffer;
        public StreamReader reader;

        public static FastLineBreakDecoderContext GetContext(IChannelHandlerContext ctx)
        {
            FastLineBreakDecoderContext obj = ctx.GetAttachment() as FastLineBreakDecoderContext;
            if (obj == null)
            {
                obj = new FastLineBreakDecoderContext();
                ctx.SetAttachment(obj);
            }
            return obj;
        }

        public FastLineBreakDecoderContext()
        {
            inputBuffer = DynamicByteBuf.GetInstance();
            reader = new StreamReader(new ByteBufReadStream().SetUnderlyingData(inputBuffer));
            outputStream = new ByteBufWriteStream();
            writer = new StreamWriter(outputStream);
            writer.AutoFlush = true;
        }

        public void Dispose()
        {
            this.reader.Dispose();
            inputBuffer.Release();
            this.reader = null;
            this.inputBuffer = null;
        }

        public ByteBufWriteStream outputStream;
        public StreamWriter writer;
    }

    public class FastLineBreakDecoder : SimpleChannelUpstreamHandler, IChannelDownstreamHandler
    {
        protected string cumulation;

        public override void MessageReceived(
                IChannelHandlerContext ctx, IMessageEvent e)
        {
            Object m = e.GetMessage();
            if (!(m is IByteBuf))
            {
                ctx.SendUpstream(e);
                return;
            }

            IByteBuf input = (IByteBuf)m;
            FastLineBreakDecoderContext decoderCtx = FastLineBreakDecoderContext.GetContext(ctx);
            DynamicByteBuf buffer = decoderCtx.inputBuffer;
            buffer.WriteBytes(input, input.ReadableBytes);

            int bufferEnd = buffer.WriterIndex;
            int index = buffer.BytesBefore((byte)'\n');
            while (index >= 0)
            {
                int lineStart = buffer.ReaderIndex;
                int lineEnd = index;
                if (lineEnd > buffer.ReaderIndex && buffer.GetByte(buffer.ReaderIndex + lineEnd - 1) == (byte)'\r')
                {
                    lineEnd -= 1;
                }
                buffer.SetIndex(buffer.ReaderIndex, buffer.ReaderIndex + lineEnd);
                String line = decoderCtx.reader.ReadToEnd();
                Channels.FireMessageReceived(ctx, line);
                buffer.SetIndex(lineStart + index + 1, bufferEnd);
                index = buffer.BytesBefore((byte)'\n');
            }
            buffer.Compact();
        }

        public void HandleDownstream(IChannelHandlerContext ctx, IChannelEvent evt)
        {
            if (!(evt is IMessageEvent))
            {
                ctx.SendDownstream(evt);
                return;
            }

            IMessageEvent e = (IMessageEvent)evt;
            Object originalMessage = e.GetMessage();
            if (originalMessage != null)
            {
                if (originalMessage is String)
                {
                    FastLineBreakDecoderContext decoderCtx = FastLineBreakDecoderContext.GetContext(ctx);
                    decoderCtx.outputStream.SetUnderlyingBuffer(DynamicByteBuf.GetInstance());
                    decoderCtx.writer.WriteLine((string)originalMessage);
                    IByteBuf buf = decoderCtx.outputStream.GetUnderlyingBuffer();
                    Channels.Write(ctx.GetChannel(), buf);
                    return;
                }
            }
            ctx.SendDownstream(evt);
        }
    }
}
