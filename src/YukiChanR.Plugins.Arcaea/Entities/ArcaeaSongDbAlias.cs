using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YukiChanR.Plugins.Arcaea.Entities;

[Table("alias")]
public sealed record ArcaeaSongDbAlias
(
    [property: Column("sid")] string SongId,
    [property: Key]
    [property: Column("alias")]
    string Alias
);