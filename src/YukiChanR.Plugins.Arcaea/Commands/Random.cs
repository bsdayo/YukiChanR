using Flandre.Core.Messaging;
using Flandre.Framework.Common;
using Flandre.Framework.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using UnofficialArcaeaAPI.Lib.Models;
using YukiChanR.Core.Utils;
using YukiChanR.Plugins.Arcaea.Entities;

namespace YukiChanR.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.random")]
    [StringShortcut("随机曲目", AllowArguments = true)]
    public async Task<MessageContent> OnRandom(MessageContext ctx, params string[] range)
    {
        var randomLocalizer = _localizer.GetSection("Random");
        int start, end;
        var allCharts = await _songDb.Charts.AsNoTracking().ToListAsync();

        switch (range.Length)
        {
            // 不提供参数，全曲 Future 难度随机
            case 0:
                return await ConstructRandomReply(ctx, allCharts
                    .Where(chart => chart.RatingClass == (int)ArcaeaDifficulty.Future)
                    .ToArray(), randomLocalizer);

            // 提供定数范围
            case 1:
                start = ArcaeaUtils.GetRatingRange(range[0]).Start;
                end = ArcaeaUtils.GetRatingRange(range[0]).End;

                if (start == -1 || end == -1)
                    return ctx.Reply(randomLocalizer["InvalidRating"]);
                if (start is < 0 or > 120 || end is < 0 or > 126)
                    return ctx.Reply(randomLocalizer["RatingOutOfRange"]);

                return await ConstructRandomReply(ctx, allCharts
                    .Where(chart => chart.Rating >= start && chart.Rating <= end)
                    .ToArray(), randomLocalizer);

            // 提供最低和最高定数
            case 2:
                start = ArcaeaUtils.GetRatingRange(range[0]).Start;
                end = ArcaeaUtils.GetRatingRange(range[1]).End;

                if (start == -1 || end == -1)
                    return ctx.Reply(randomLocalizer["InvalidRating"]);
                if (start > end)
                    return ctx.Reply(randomLocalizer["StartLargerThenEnd"]);
                if (start is < 0 or > 120 || end is < 0 or > 120)
                    return ctx.Reply(randomLocalizer["RatingOutOfRange"]);

                return await ConstructRandomReply(ctx, allCharts
                    .Where(chart => chart.Rating >= start && chart.Rating <= end)
                    .ToArray(), randomLocalizer);

            default:
                return ctx.Reply(_commonLocalizer["ArgumentTooMuch"]);
        }
    }

    private async Task<MessageBuilder> ConstructRandomReply(MessageContext ctx, ArcaeaSongDbChart[] allCharts,
        IStringLocalizer<ArcaeaPlugin> randomLocalizer)
    {
        if (allCharts.Length == 0)
            return ctx.Reply(randomLocalizer["NoSongInRange"]);

        var chart = allCharts[new Random().Next(allCharts.Length)];

        var songCover = await _cacheManager.GetSongCoverAsync(
            chart.SongId, chart.JacketOverride, (ArcaeaDifficulty)chart.RatingClass);
        var package = await _songDb.Packages
            .AsNoTracking()
            .FirstAsync(package => package.Set == chart.Set);
        var rating = chart.Rating / 10d;

        return ctx.Reply()
            .Text(randomLocalizer["ReplyTitle"])
            .Image(songCover)
            .Text(randomLocalizer["ReplyBody",
                chart.NameEn,
                package.Name,
                (ArcaeaDifficulty)chart.RatingClass,
                rating.ToDisplayRating(), rating]);
    }
}