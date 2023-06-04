using Flandre.Core.Messaging;
using Flandre.Framework.Common;
using Flandre.Framework.Routing;
using YukiChanR.Core.Utils;

namespace YukiChanR.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.preview")]
    [StringShortcut("查预览", AllowArguments = true)]
    [StringShortcut("查谱面", AllowArguments = true)]
    [StringShortcut("查铺面", AllowArguments = true)]
    public async Task<MessageContent> OnPreview(MessageContext ctx, params string[] songnameAndDifficulty)
    {
        var (songname, difficulty) = ArcaeaUtils.ParseMixedSongNameAndDifficulty(songnameAndDifficulty);

        var songId = await _songDb.SearchIdAsync(songname);
        if (songId is null) return ctx.Reply("没有找到该曲目呢...");

        var preview = await _cacheManager.GetPreviewImageAsync(songId, difficulty);

        return ctx.Reply().Image(preview);
    }
}