using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YukiChanR.Plugins.Arcaea.Entities;

[Table("arcaea_users")]
public sealed class ArcaeaDatabaseUser
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("platform")]
    public required string Platform { get; set; }

    [Column("user_id")]
    public required string UserId { get; set; }

    [Column("arcaea_code")]
    public required string ArcaeaCode { get; set; }

    [Column("arcaea_name")]
    public string ArcaeaName { get; set; } = string.Empty;
}