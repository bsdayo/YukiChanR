using System.Text;
using Flandre.Core.Messaging;
using Flandre.Core.Messaging.Segments;
using Flandre.Framework.Common;
using Flandre.Framework.Routing;
using UnofficialArcaeaAPI.Lib.Models;
using YukiChanR.Core.Utils;

namespace YukiChanR.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.info")]
    [StringShortcut("查定数", AllowArguments = true)]
    public async Task<MessageContent> OnInfo(MessageContext ctx,
        [Option(ShortName = 'n')] bool nya,
        params string[] songname)
    {
        var infoLocalizer = _localizer.GetSection("Info");
        var song = await _songDb.SearchSongAsync(string.Join("", songname));

        if (song is null)
            return ctx.Reply(_commonLocalizer["SongNotFound"]);

        var cover = await _cacheManager.GetSongCoverAsync(song.SongId);

        var mb = new MessageBuilder().Image(ImageSegment.FromData(cover));

        if (song.Difficulties.Length > 3 && song.Difficulties[3].JacketOverride)
        {
            var bydCover = await _cacheManager.GetSongCoverAsync(
                song.SongId, true, ArcaeaDifficulty.Beyond, nya);
            mb.Image(ImageSegment.FromData(bydCover));
        }

        var sb = new StringBuilder();
        for (var i = 0; i < song.Difficulties.Length; i++)
        {
            var rating = song.Difficulties[i].Rating;
            sb.Append(_localizer["Info:Difficulty", (ArcaeaDifficulty)i, rating.ToDisplayRating(), rating]);
        }

        return new MessageBuilder()
            .Image(cover)
            .Text(infoLocalizer["Reply",
                song.Difficulties[2].NameEn,
                song.SetFriendly,
                sb.ToString()].ToString());
    }
}