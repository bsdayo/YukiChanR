// ReSharper disable InconsistentNaming

using UnofficialArcaeaAPI.Lib.Models;

#pragma warning disable CS8618

namespace YukiChanR.Plugins.Arcaea.Models;

public sealed class ArcaeaRecord
{
    public string Name { get; init; }

    public string SongId { get; init; }

    public double Potential { get; init; }

    public double Rating { get; init; }

    public ArcaeaDifficulty Difficulty { get; init; }

    public int Score { get; init; }

    public int ShinyPureCount { get; init; }

    public int PureCount { get; init; }

    public int FarCount { get; init; }

    public int LostCount { get; init; }

    public ArcaeaClearType ClearType { get; init; }

    public ArcaeaGrade Grade { get; init; }

    public int RecollectionRate { get; init; }

    public bool JacketOverride { get; init; }

    public DateTime PlayTime { get; init; }

    public static ArcaeaRecord FromUaa(UaaRecord record, UaaChartInfo chartInfo)
    {
        return new ArcaeaRecord
        {
            Name = chartInfo.NameEn,
            SongId = record.SongId,
            Potential = record.Rating,
            Difficulty = (ArcaeaDifficulty)record.Difficulty,
            Rating = chartInfo.Rating / 10d,
            Score = record.Score,
            ClearType = (ArcaeaClearType)record.ClearType!,
            Grade = ArcaeaUtils.GetGrade(record.Score),
            //
            ShinyPureCount = record.ShinyPerfectCount,
            PureCount = record.PerfectCount,
            FarCount = record.NearCount,
            LostCount = record.MissCount,
            RecollectionRate = record.Health!.Value,
            //
            JacketOverride = chartInfo.JacketOverride,
            PlayTime = DateTimeOffset.FromUnixTimeMilliseconds(record.TimePlayed).UtcDateTime
        };
    }
}