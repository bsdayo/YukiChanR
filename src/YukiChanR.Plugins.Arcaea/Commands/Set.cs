using Flandre.Core.Messaging;
using Flandre.Framework.Common;
using Flandre.Framework.Routing;
using YukiChanR.Core.Utils;
using YukiChanR.Plugins.Arcaea.Entities;

namespace YukiChanR.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.set")]
    public async Task<MessageContent> OnSet(CommandContext ctx, params string[] preferences)
    {
        var pref = await _database.Preferences
            .FirstOrAddAsync(
                p => p.Platform == ctx.Platform && p.UserId == ctx.UserId,
                () => new ArcaeaUserPreferences { Platform = ctx.Platform, UserId = ctx.UserId });

        bool ParseBoolSet(string[] set)
            => set.Length <= 1 || !set[1].Equals("false", StringComparison.OrdinalIgnoreCase);

        foreach (var prefStr in preferences)
        {
            var set = prefStr.Split('=');
            switch (set[0].ToLower().Replace("_", ""))
            {
                case "dark":
                    pref.Dark = ParseBoolSet(set);
                    break;

                case "nya":
                    pref.Nya = ParseBoolSet(set);
                    break;

                case "singledynamicbg":
                    pref.SingleDynamicBackground = ParseBoolSet(set);
                    break;

                case "b30showgrade":
                    pref.Best30ShowGrade = ParseBoolSet(set);
                    break;
            }
        }

        await _database.SaveChangesAsync();
        return ctx.Reply("已成功为您更新偏好信息。");
    }
}