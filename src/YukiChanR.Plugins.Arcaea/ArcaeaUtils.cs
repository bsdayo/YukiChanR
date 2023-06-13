using System.Text;
using UnofficialArcaeaAPI.Lib.Models;
using YukiChanR.Plugins.Arcaea.Models;

namespace YukiChanR.Plugins.Arcaea;

public static class ArcaeaUtils
{
    public static bool ValidateUserCode(string input)
    {
        return input.Length == 9 && int.TryParse(input, out _);
    }
    
    /// <summary>
    /// 转换 rating (eg. 98) 为难度 (eg. 9+)
    /// </summary>
    /// <param name="rating">曲目 rating (定数*10)</param>
    /// <returns>难度文本</returns>
    public static string ToDisplayRating(this double rating)
    {
        var major = (int)rating;
        return rating > 9 && rating - major >= 0.7
            ? major + "+"
            : major.ToString();
    }

    public static string ToDisplayPotential(double potential)
    {
        return potential >= 0 ? potential.ToString("F2") : "?";
    }

    public static string ToDisplayScore(this int score)
    {
        var span = score.ToString("00000000").AsSpan();
        var sb = new StringBuilder();
        sb.Append(span[..2]).Append('\'').Append(span[2..5]).Append('\'').Append(span[5..]);
        return sb.ToString();
    }

    public static string ToShortDisplayDifficulty(this ArcaeaDifficulty difficulty)
    {
        return difficulty switch
        {
            ArcaeaDifficulty.Past => "PST",
            ArcaeaDifficulty.Present => "PRS",
            ArcaeaDifficulty.Future => "FTR",
            ArcaeaDifficulty.Beyond => "BYD",
            _ => $"<{(int)difficulty}>"
        };
    }

    /// <summary>
    /// 转换难度文本 (eg. 9+) 为难度区间 (eg. 97 ~ 99)
    /// </summary>
    /// <param name="difficulty">难度文本</param>
    /// <returns>难度区间</returns>
    public static (int Start, int End) GetRatingRange(string difficulty)
    {
        return difficulty switch
        {
            "1" => (10, 19),
            "2" => (20, 29),
            "3" => (30, 39),
            "4" => (40, 49),
            "5" => (50, 59),
            "6" => (60, 69),
            "7" => (70, 79),
            "8" => (80, 89),
            "9" => (90, 96),
            "9+" => (97, 99),
            "10" => (100, 106),
            "10+" => (107, 109),
            "11" => (110, 116),
            "11+" => (117, 119),
            "12" => (120, 126),
            _ => double.TryParse(difficulty, out var rating)
                ? ((int)(rating * 10), (int)(rating * 10))
                : (-1, -1)
        };
    }

    public static ArcaeaDifficulty? GetArcaeaDifficulty(string difficultyText)
    {
        return difficultyText.ToLower() switch
        {
            "0" or "pst" or "past" => ArcaeaDifficulty.Past,
            "1" or "prs" or "present" => ArcaeaDifficulty.Present,
            "2" or "ftr" or "future" => ArcaeaDifficulty.Future,
            "3" or "byd" or "byn" or "beyond" => ArcaeaDifficulty.Beyond,
            _ => null
        };
    }

    public static ArcaeaGrade GetGrade(int score)
    {
        return score switch
        {
            >= 9900000 => ArcaeaGrade.EXP,
            >= 9800000 and < 9900000 => ArcaeaGrade.EX,
            >= 9500000 and < 9800000 => ArcaeaGrade.AA,
            >= 9200000 and < 9500000 => ArcaeaGrade.A,
            >= 8900000 and < 9200000 => ArcaeaGrade.B,
            >= 8600000 and < 8900000 => ArcaeaGrade.C,
            _ => ArcaeaGrade.D
        };
    }

    public static double CalculatePotential(double rating, int score)
    {
        var ptt = score switch
        {
            >= 10000000 => rating + 2,
            >= 9800000 => rating + 1 + ((double)score - 9800000) / 200000,
            _ => rating + ((double)score - 9500000) / 300000
        };

        return Math.Max(0, ptt);
    }

    public static (string, ArcaeaDifficulty) ParseMixedSongNameAndDifficulty(string[] mixed)
    {
        var difficulty = GetArcaeaDifficulty(mixed[^1]);
        var songname = string.Join(' ', difficulty is null ? mixed : mixed[..^1]);
        return (songname, difficulty ?? ArcaeaDifficulty.Future);
    }

    public static ArcaeaGuessMode? GetGuessMode(string text)
    {
        return text.ToLower() switch
        {
            "e" or "easy" or "简单"
                => ArcaeaGuessMode.Easy,

            "n" or "normal" or "正常"
                => ArcaeaGuessMode.Normal,

            "h" or "hard" or "困难"
                => ArcaeaGuessMode.Hard,

            "f" or "flash" or "闪照"
                => ArcaeaGuessMode.Flash,

            "g" or "gray" or "grayscale" or "灰度"
                => ArcaeaGuessMode.GrayScale,

            "i" or "invert" or "反色"
                => ArcaeaGuessMode.Invert,

            _ => null
        };
    }

    public static string GetTitle(this ArcaeaGuessMode mode)
    {
        return mode switch
        {
            ArcaeaGuessMode.Easy => "简单",
            ArcaeaGuessMode.Normal => "正常",
            ArcaeaGuessMode.Hard => "困难",
            ArcaeaGuessMode.Flash => "闪照",
            ArcaeaGuessMode.GrayScale => "灰度",
            ArcaeaGuessMode.Invert => "反色",
            _ => "未知模式"
        };
    }
}