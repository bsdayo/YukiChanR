using System.Text;
using SkiaSharp;

namespace YukiChanR.Plugins.Arcaea.ImageGen;

public static class ArcaeaImageUtils
{
    /// <summary>
    /// eg. "123456789" -> "123 456 789"
    /// </summary>
    public static string GetSpacedUserCode(string usercode)
    {
        var span = usercode.AsSpan();
        var sb = new StringBuilder();
        return sb
            .Append(span[..3])
            .Append(' ')
            .Append(span[3..6])
            .Append(' ')
            .Append(span[6..])
            .ToString();
    }

    public static string ReplaceNotSupportedChar(string text)
    {
        return text
            .Replace('：', ':')
            .Replace('α', 'a')
            .Replace('β', 'b')
            .Replace('έ', 'e')
            .Replace('ό', 'o')
            .Replace('γ', 'g')
            .Replace('Ä', 'A')
            .Replace('ö', 'o')
            .Replace('δ', 'd')
            .Replace('ω', 'w')
            .Replace('ο', 'o')
            .Replace('κ', 'k');
    }

    public static void DrawLimitedText(this SKCanvas canvas, string text,
        float x, float y, SKPaint paint, float widthLimit)
    {
        var originalWidth = paint.MeasureText(text);
        paint.TextScaleX = originalWidth > widthLimit ? widthLimit / originalWidth : 1;
        canvas.DrawText(text, x, y, paint);
    }
}