using YukiChanR.Core.Utils;
using YukiChanR.Plugins.Arcaea.Models;

namespace YukiChanR.Plugins.Arcaea;

public sealed class ArcaeaResourceManager : YukiResourceManager
{
    public ArcaeaResourceManager() : base(typeof(ArcaeaResourceManager).Assembly, "Resources")
    {
    }

    public byte[] GetImageData(string filename) => GetData($"Images.{filename}");

    public Stream GetFontStream(string filename) => GetStream($"Fonts.{filename}");

    // ---

    public byte[] GetDefaultCover() => GetImageData("song-cover-placeholder.png");

    public byte[] GetClearTypeImage(ArcaeaClearType clearType)
    {
        var filename = clearType switch
        {
            ArcaeaClearType.NormalClear => "clear-tc.png",
            ArcaeaClearType.EasyClear => "clear-tc.png",
            ArcaeaClearType.HardClear => "clear-tc.png",
            ArcaeaClearType.TrackLost => "clear-tl.png",
            ArcaeaClearType.FullRecall => "clear-fr.png",
            ArcaeaClearType.PureMemory => "clear-pm.png",
            _ => "clear-tc.png"
        };
        return GetImageData(filename);
    }

    public byte[] GetMiniClearTypeImage(ArcaeaClearType clearType)
    {
        var filename = clearType switch
        {
            ArcaeaClearType.NormalClear => "clear-mini-nc.png",
            ArcaeaClearType.EasyClear => "clear-mini-ec.png",
            ArcaeaClearType.HardClear => "clear-mini-hc.png",
            ArcaeaClearType.TrackLost => "clear-mini-tl.png",
            ArcaeaClearType.FullRecall => "clear-mini-fr.png",
            ArcaeaClearType.PureMemory => "clear-mini-pm.png",
            _ => "clear-mini-nc.png"
        };
        return GetImageData(filename);
    }

    public byte[] GetGradeImage(ArcaeaGrade grade)
    {
        var filename = $"grade-{grade.ToString().ToLower()}.png";
        return GetImageData(filename);
    }

    public byte[] GetMiniGradeImage(ArcaeaGrade grade)
    {
        var filename = $"grade-mini-{grade.ToString().ToLower()}.png";
        return GetImageData(filename);
    }
}