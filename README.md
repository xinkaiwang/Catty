# Catty
C# asynchronous event-driven network application framework inspired by Netty

Catty is a Ansync I/O server/client  framework which enables quick and easy development 
of network applications such as protocol servers and clients. It greatly simplifies 
and streamlines network programming such as TCP socket server.


##Example 1: Discard Service (RFC863)

```C#
public class MyHandler : SimpleChannelUpstreamHandler
{
    public override void MessageReceived(IChannelHandlerContext ctx, IMessageEvent e)
    {
        // do nothing
    }
}
public class Program
{
    public static void Main(string[] args)
    {
        BasicConfigurator.Configure(); // config log4net
        Func<IChannelHandler[]> handlersFactory = () => new IChannelHandler[] {new MyHandler()};
        var server = new SimpleTcpService().SetHandlers(handlersFactory);
        server.Bind(new IPEndPoint(IPAddress.Any, 8002));
        Console.ReadLine();
    }
}
```

## Example 2: IByteBuf Echo Service
```C#
public class MyHandler : SimpleChannelUpstreamHandler
{
    public override void MessageReceived(IChannelHandlerContext ctx, IMessageEvent e)
    {
        object msg = e.GetMessage();
        Channels.Write(ctx.GetChannel(), msg);
    }
}
```

## Example 3: LineBreanDecoder + String.ToUpper() Echo Service
###Step 1: Hook up a LineBreanDecoder in the chain to do the line-break + decode work
```C#
Func<IChannelHandler[]> handlersFactory = () => new IChannelHandler[] {new LineBreakDecoder(), new MyHandler()};
```
###Step 2: now the message we see in MyHandler becomes String instead of IByteBuf
```C#
public class MyHandler : SimpleChannelUpstreamHandler
{
    public override void MessageReceived(IChannelHandlerContext ctx, IMessageEvent e)
    {
        String msg = e.GetMessage() as String;
        if (msg != null)
        {
            msg = msg.ToUpper();
            Console.WriteLine(msg);
            Channels.Write(ctx.GetChannel(), msg);
        }
    }
}
```
