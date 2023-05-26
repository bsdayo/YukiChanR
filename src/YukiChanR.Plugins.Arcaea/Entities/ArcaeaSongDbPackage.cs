using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YukiChanR.Plugins.Arcaea.Entities;

[Table("packages")]
public sealed record ArcaeaSongDbPackage
(
    [property: Key]
    [property: Column("id")]
    string Set,
    [property: Column("name")] string Name
);