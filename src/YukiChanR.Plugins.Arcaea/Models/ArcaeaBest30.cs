using System.Text.Json.Serialization;
using UnofficialArcaeaAPI.Lib.Responses;

#pragma warning disable CS8618

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace YukiChanR.Plugins.Arcaea.Models;

public sealed class ArcaeaBest30
{
    public ArcaeaUser User { get; set; }

    public double Recent10Avg { get; set; }

    public double Best30Avg { get; set; }

    public ArcaeaRecord[] Records { get; set; }

    public ArcaeaRecord[]? OverflowRecords { get; set; }

    [JsonIgnore]
    public bool HasOverflow => OverflowRecords is not null;

    public static ArcaeaBest30 FromUaa(UaaUserBestsResultContent result)
    {
        return new ArcaeaBest30
        {
            User = ArcaeaUser.FromUaa(result.AccountInfo),

            Recent10Avg = result.Recent10Avg,
            Best30Avg = result.Best30Avg,

            Records = result.Best30List.Select((record, i)
                    => ArcaeaRecord.FromUaa(record, result.Best30SongInfo![i]))
                .ToArray(),

            OverflowRecords = result.Best30Overflow is null
                ? Array.Empty<ArcaeaRecord>()
                : result.Best30Overflow.Select((record, i)
                        => ArcaeaRecord.FromUaa(record, result.Best30OverflowSongInfo![i]))
                    .ToArray()
        };
    }
}