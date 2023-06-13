using Flandre.Core.Messaging;
using Flandre.Framework.Routing;
using UnofficialArcaeaAPI.Lib.Models;
using YukiChanR.Core.Utils;

namespace YukiChanR.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.ptt")]
    public async Task<MessageContent> OnPtt(MessageContext ctx, params string[] args)
    {
        var pttLocalizer = _localizer.GetSection("Ptt");
        var difficulty = ArcaeaDifficulty.Future;
        string songname;
        var score = 0;

        switch (args)
        {
            case []:
                return ctx.Reply(pttLocalizer["NeedQuery"]);

            case [_]:
                return ctx.Reply(pttLocalizer["NeedQuery"]);

            case [var songStr, var scoreStr]:
                songname = songStr;
                if (!int.TryParse(scoreStr, out score))
                    return ctx.Reply(pttLocalizer["InvalidScore"]);
                break;

            default:
                var scoreIndex = Array.FindIndex(args, a => int.TryParse(a, out score));
                if (scoreIndex < 0) return ctx.Reply(pttLocalizer["InvalidScore"]);
                songname = string.Join(' ', args[..scoreIndex]);
                if (args.Length <= scoreIndex + 1) break;
                var diff = ArcaeaUtils.GetArcaeaDifficulty(args[scoreIndex + 1]);
                if (diff is null)
                    return ctx.Reply(pttLocalizer["InvalidDifficulty"]);
                difficulty = diff.Value;
                break;
        }

        var song = await _songDb.SearchSongAsync(songname);
        if (song is null)
            return ctx.Reply(_commonLocalizer["SongNotFound"]);

        var rating = song.Difficulties[(int)difficulty].Rating;

        var ptt = ArcaeaUtils.CalculatePotential(rating, score);

        return ctx.Reply(pttLocalizer["Reply",
            song.Difficulties[(int)difficulty].NameEn,
            difficulty, score, ptt]);
    }
}