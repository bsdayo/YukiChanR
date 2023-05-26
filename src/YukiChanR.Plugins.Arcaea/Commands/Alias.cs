using Flandre.Core.Messaging;
using Flandre.Framework.Attributes;
using Flandre.Framework.Common;
using Microsoft.EntityFrameworkCore;
using YukiChanR.Core.Utils;

// ReSharper disable CheckNamespace

namespace YukiChanR.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.alias")]
    [StringShortcut("查别名", AllowArguments = true)]
    public async Task<MessageContent> OnAlias(CommandContext ctx, params string[] songname)
    {
        var query = string.Join(' ', songname);

        var song = await _songDb.SearchSongAsync(query);
        if (song is null)
            // return ctx.Reply("没有找到该曲目哦~");
            return ctx.Reply(_localizer["SongNotFound"]);

        var aliases = await _songDb.Aliases
            .AsNoTracking()
            .Where(alias => alias.SongId == song.SongId)
            .Select(alias => alias.Alias)
            .ToListAsync();

        return ctx.Reply()
            .Text($"{song.Difficulties[2].NameEn} - {song.Difficulties[2].Artist}\n")
            .Text("可用的别名有：\n")
            .Text(string.Join('\n', aliases));
    }
}