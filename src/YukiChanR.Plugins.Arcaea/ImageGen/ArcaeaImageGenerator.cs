using SkiaSharp;

namespace YukiChanR.Plugins.Arcaea.ImageGen;

public sealed partial class ArcaeaImageGenerator
{
    private readonly ArcaeaCacheManager _cacheManager;

    private readonly ArcaeaResourceManager _resources;

    private readonly (
        SKTypeface TitilliumWeb_Regular,
        SKTypeface TitilliumWeb_SemiBold,
        SKTypeface GeoSans) _fonts;

    private readonly List<(
        string ColorLight, string ColorDark,
        string ColorBorderLight, string ColorBorderDark,
        string ColorInnerLight, string ColorInnerDark)> _difficultyColors = new()
    {
        ("#51a9c8", "#4188a1", "#71c9e8", "#61a8c1", "#3189a8", "#216881"), // Past
        ("#a8c96b", "#87a256", "#c8e98b", "#a7c276", "#88a94b", "#678236"), // Present
        ("#8a4876", "#6f3a5f", "#aa6896", "#8f5a7f", "#6a2856", "#4f1a3f"), // Future
        ("#bb2b43", "#962336", "#db4b63", "#b64356", "#9b0b23", "#760316") // Beyond
    };

    public ArcaeaImageGenerator(ArcaeaCacheManager cacheManager, ArcaeaResourceManager resources)
    {
        _cacheManager = cacheManager;
        _resources = resources;

        _fonts = (
            SKTypeface.FromStream(_resources.GetFontStream("TitilliumWeb-Regular.ttf")),
            SKTypeface.FromStream(_resources.GetFontStream("TitilliumWeb-SemiBold.ttf")),
            SKTypeface.FromStream(_resources.GetFontStream("GeosansLight.ttf")));
    }
}