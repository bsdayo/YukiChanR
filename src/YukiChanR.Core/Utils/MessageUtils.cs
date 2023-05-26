using Flandre.Core.Messaging;
using Flandre.Core.Messaging.Segments;

namespace YukiChanR.Core.Utils;

public static class MessageUtils
{
    public static MessageBuilder Reply(this MessageContext ctx, string? message = null)
    {
        var builder = new MessageBuilder();
        if (ctx.Platform.Equals("qqguild", StringComparison.OrdinalIgnoreCase))
            builder.Add(new AtSegment(ctx.UserId));
        else
            builder.Add(new QuoteSegment(ctx.Message));

        if (message is not null)
            builder.Text(message);

        return builder;
    }
}