using System.Diagnostics;
using System.Text;
using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using YukiChanR.Core.Utils;

namespace YukiChanR.Plugins.Monitor;

public sealed class MonitorPlugin : Plugin
{
    public static Stopwatch UptimeStopwatch { get; } = new();

    [Command("monitor", "m")]
    public static MessageContent Monitor(CommandContext ctx,
        [Option(ShortName = 'a')] bool all = false,
        [Option(ShortName = 'u')] bool uptime = false
    )
    {
        var builder = new StringBuilder();
        builder.AppendLine("暮雪酱 Revive | YukiChanR");
        builder.AppendLine($"[{BuildInfo.Branch}@{BuildInfo.ShortCommitHash}]");

        if (uptime || all)
        {
            builder.AppendLine($"\nUptime: {UptimeStopwatch.Elapsed:d\\.hh\\:mm\\:ss\\.fff}");
        }

        builder.Append("\nDeveloped with love by bsdayo.");

        return ctx.Reply(builder.ToString());
    }
}