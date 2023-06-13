using Flandre.Core.Messaging;
using Flandre.Framework.Common;
using Flandre.Framework.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UnofficialArcaeaAPI.Lib.Models;
using YukiChanR.Core.Utils;
using YukiChanR.Plugins.Arcaea.Entities;
using YukiChanR.Plugins.Arcaea.Models;

namespace YukiChanR.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.best")]
    [StringShortcut("查最高", AllowArguments = true)]
    public async Task<MessageContent> OnBest(MessageContext ctx,
        [Option(ShortName = 'n')] bool nya,
        [Option(ShortName = 'd')] bool dark,
        [Option(ShortName = 'u')] string user,
        params string[] songnameAndDifficulty)
    {
        var bestLocalizer = _localizer.GetSection("Best");
        var (songQuery, difficulty) = ArcaeaUtils.ParseMixedSongNameAndDifficulty(songnameAndDifficulty);

        string target;
        string logTarget;

        if (string.IsNullOrEmpty(user))
        {
            var binding = await _database.Users.AsNoTracking().FirstOrDefaultAsync(
                u => u.Platform == ctx.Platform && u.UserId == ctx.UserId);
            if (binding is null)
                return ctx.Reply(_commonLocalizer["UserNotBound", "/a best -u 用户名或好友码"]);
            target = binding.ArcaeaCode;
            logTarget = binding.ArcaeaName;
        }
        else
        {
            logTarget = target = user;
        }

        var song = await _songDb.SearchSongAsync(songQuery);
        if (song is null)
            return ctx.Reply(_commonLocalizer["SongNotFound"]);

        var songname = song.Difficulties[(int)difficulty].NameEn;

        _logger.LogInformation("[Best:Query] {ArcaeaUser} -> {SongName} ({Difficulty})",
            logTarget, songname, difficulty.ToShortDisplayDifficulty());

        var bestInfo = await _uaaService.UaaClient.User.GetBestAsync(
            target, song.SongId, UaaSongQueryType.SongId, difficulty, UaaReplyWith.SongInfo);
        var userInfo = ArcaeaUser.FromUaa(bestInfo.AccountInfo);

        var pref = await _database.Preferences.AsNoTracking().FirstOrDefaultAsync(
                       p => p.Platform == ctx.Platform && p.UserId == ctx.UserId)
                   ?? new ArcaeaUserPreferences();

        pref.Nya = pref.Nya || nya;
        pref.Dark = pref.Dark || dark;

        _logger.LogInformation("[Best:ImageGen] {ArcaeaUser} -> {SongName} ({Difficulty})",
            logTarget, songname, difficulty.ToShortDisplayDifficulty());

        var image = await _imageGen.GenerateSingleAsync(
            ArcaeaRecord.FromUaa(bestInfo.Record, bestInfo.SongInfo![0]), pref);

        return ctx
            .Reply(bestLocalizer["Reply",
                userInfo.Name, ArcaeaUtils.ToDisplayPotential(userInfo.Potential)])
            .Image(image);
    }
}