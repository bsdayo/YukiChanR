using Flandre.Framework;
using YukiChanR.Core.Utils;

namespace YukiChanR;

public static class Middlewares
{
    public static void UseExceptionFeedback(this FlandreApp app)
    {
        app.Use(async (ctx, next) =>
        {
            await next();

            if (ctx is { Exception: { } ex, Response: null })
                ctx.Response = ctx.Reply($"发生了奇怪的错误！({ex.GetType().Name}: {ex.Message})");
        });
    }
}