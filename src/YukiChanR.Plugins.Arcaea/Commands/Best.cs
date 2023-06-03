using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
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
        var (songQuery, difficulty) = ArcaeaUtils.ParseMixedSongNameAndDifficulty(songnameAndDifficulty);

        string target;
        string logTarget;

        if (string.IsNullOrEmpty(user))
        {
            var binding = await _database.Users.AsNoTracking().FirstOrDefaultAsync(
                u => u.Platform == ctx.Platform && u.UserId == ctx.UserId);
            if (binding is null)
                return ctx.Reply("请先使用 /a bind 名称或好友码 绑定你的账号哦~"
                                 + "你也可以直接使用 /a best -u 用户名或好友码 查询指定用户。");
            target = binding.ArcaeaCode;
            logTarget = binding.ArcaeaName;
        }
        else
        {
            logTarget = target = user;
        }

        var song = await _songDb.SearchSongAsync(songQuery);
        if (song is null)
            return ctx.Reply("没有找到该曲目哦~");

        var songname = song.Difficulties[(int)difficulty].NameEn;

        _logger.LogInformation("[Best:Query] {ArcaeaUser} -> {SongName} ({Difficulty})",
            logTarget, songname, difficulty.ToShortDisplayDifficulty());

        var bestInfo = await _uaaClient.User.GetBestAsync(
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
            .Reply($"{userInfo.Name} ({ArcaeaUtils.ToDisplayPotential(userInfo.Potential)})\n")
            .Image(image);
    }
}