using Flandre.Framework;
using Microsoft.Extensions.Logging;
using Sentry;
using UnofficialArcaeaAPI.Lib;
using YukiChanR.Core.Utils;

namespace YukiChanR;

public static class Middlewares
{
    public static void UseExceptionFeedback(this FlandreApp app)
    {
        if (SentrySdk.IsEnabled)
        {
            AppDomain.CurrentDomain.UnhandledException += (_, args) =>
                SentrySdk.CaptureException((Exception)args.ExceptionObject);
            TaskScheduler.UnobservedTaskException += (_, args) =>
                SentrySdk.CaptureException(args.Exception);
        }
        else
        {
            app.Logger.LogWarning("Configuration Sentry:Dsn is not set, Sentry related features disabled");
        }

        app.Use(async (ctx, next) =>
        {
            await next();

            if (ctx.Exception is { } ex)
            {
                ctx.Response ??= ctx.Reply($"发生了奇怪的错误！({ex.GetType().Name}: {ex.Message})");

                if (SentrySdk.IsEnabled)
                {
                    if (ctx.Exception is UaaException) return;
                    new Task(() => SentrySdk.CaptureException(ctx.Exception)).Start();
                }
            }
        });
    }
}