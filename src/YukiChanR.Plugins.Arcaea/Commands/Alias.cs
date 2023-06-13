using System.Text;
using Flandre.Core.Messaging;
using Flandre.Framework.Common;
using Flandre.Framework.Routing;
using Microsoft.EntityFrameworkCore;
using YukiChanR.Core.Utils;

namespace YukiChanR.Plugins.Arcaea;

public partial class ArcaeaPlugin
{
    [Command("a.alias")]
    [StringShortcut("查别名", AllowArguments = true)]
    public async Task<MessageContent> OnAlias(CommandContext ctx, params string[] songname)
    {
        var aliasLocalizer = _localizer.GetSection("Alias");
        var query = string.Join(' ', songname);

        var song = await _songDb.SearchSongAsync(query);
        if (song is null)
            return ctx.Reply(_commonLocalizer["SongNotFound"]);

        var aliases = await _songDb.Aliases
            .AsNoTracking()
            .Where(alias => alias.SongId == song.SongId)
            .Select(alias => alias.Alias)
            .ToListAsync();

        var sb = new StringBuilder();
        foreach (var alias in aliases)
            sb.Append($"\n- {alias}");

        return ctx.Reply()
            .Text(aliasLocalizer["Reply",
                song.Difficulties[2].NameEn,
                song.Difficulties[2].Artist,
                sb.ToString()]);
    }
}