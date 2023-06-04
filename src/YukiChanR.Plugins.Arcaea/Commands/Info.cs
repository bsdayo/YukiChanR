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
        var song = await _songDb.SearchSongAsync(string.Join("", songname));

        // TODO: query from server

        if (song is null)
            return ctx.Reply("没有找到该曲目哦~");

        var cover = await _cacheManager.GetSongCoverAsync(song.SongId);

        var mb = new MessageBuilder().Image(ImageSegment.FromData(cover));

        if (song.Difficulties.Length > 3 && song.Difficulties[3].JacketOverride)
        {
            var bydCover = await _cacheManager.GetSongCoverAsync(
                song.SongId, true, ArcaeaDifficulty.Beyond, nya);
            mb.Image(ImageSegment.FromData(bydCover));
        }

        mb
            .Text(song.Difficulties[2].NameEn + "\n")
            .Text($"({song.SetFriendly})");

        for (var i = 0; i < song.Difficulties.Length; i++)
        {
            var rating = song.Difficulties[i].Rating;
            mb.Text($"\n{(ArcaeaDifficulty)i} {rating.ToDisplayRating()} [{rating:N1}]");
        }

        return mb;
    }
}