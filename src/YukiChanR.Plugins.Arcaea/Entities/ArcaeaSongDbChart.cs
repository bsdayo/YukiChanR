using System.ComponentModel.DataAnnotations.Schema;

namespace YukiChanR.Plugins.Arcaea.Entities;

/// <remarks>
/// 主键是 song_id 和 rating_class，在 <see cref="ArcaeaSongDbContext"/> 中使用 Fluent API 定义
/// </remarks>
[Table("charts")]
public sealed record ArcaeaSongDbChart(
    [property: Column("song_id")] string SongId,
    [property: Column("rating_class")] int RatingClass,
    [property: Column("set")] string Set,
    [property: Column("name_en")] string NameEn,
    [property: Column("name_jp")] string NameJp,
    [property: Column("artist")] string Artist,
    [property: Column("bpm")] string Bpm,
    [property: Column("bpm_base")] double BpmBase,
    [property: Column("time")] int Time,
    [property: Column("side")] int Side,
    [property: Column("world_unlock")] bool WorldUnlock,
    [property: Column("remote_download")] bool RemoteDownload,
    [property: Column("bg")] string Bg,
    [property: Column("date")] long Date,
    [property: Column("version")] string Version,
    [property: Column("difficulty")] int Difficulty,
    [property: Column("rating")] int Rating,
    [property: Column("note")] int Note,
    [property: Column("chart_designer")] string ChartDesigner,
    [property: Column("jacket_designer")] string JacketDesigner,
    [property: Column("jacket_override")] bool JacketOverride,
    [property: Column("audio_override")] bool AudioOverride
);