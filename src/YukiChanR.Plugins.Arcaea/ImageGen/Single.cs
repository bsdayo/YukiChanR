using SkiaSharp;
using YukiChanR.Plugins.Arcaea.Entities;
using YukiChanR.Plugins.Arcaea.Models;

namespace YukiChanR.Plugins.Arcaea.ImageGen;

public partial class ArcaeaImageGenerator
{
    public async Task<byte[]> GenerateSingleAsync(ArcaeaRecord record, ArcaeaUserPreferences pref)
    {
        var imageInfo = new SKImageInfo(900, 1520);
        using var surface = SKSurface.Create(imageInfo);
        using var canvas = surface.Canvas;

        var cover = await _cacheManager.GetSongCoverAsync(
            record.SongId, record.JacketOverride, record.Difficulty, pref.Nya);

        var (colorLight, colorDark, colorBorderLight, colorBorderDark, colorInnerLight, colorInnerDark)
            = _difficultyColors[(int)record.Difficulty];

        if (pref.SingleDynamicBackground)
        {
            using var bgBitmap =
                SKBitmap.Decode(await GetSingleV1Background(record))!;
            canvas.DrawBitmap(bgBitmap, 0, 0);
        }
        else
        {
            var bgImage = _resources.GetImageData("single-background.jpg");
            using var background = SKBitmap.Decode(bgImage);

            using var scaledBackground = new SKBitmap(900, 1520);
            background?.ScalePixels(scaledBackground, SKFilterQuality.Medium);

            canvas.DrawBitmap(scaledBackground, 0, 0);
        }

        // 背景
        {
            using var cardPaint = new SKPaint
            {
                Color = pref.Dark
                    ? new SKColor(40, 40, 40, 240)
                    : new SKColor(255, 255, 255, 200),
                IsAntialias = true,
                ImageFilter = SKImageFilter.CreateDropShadow(
                    0, 0, 35, 35, new SKColor(0, 0, 0, 50))
            };
            canvas.DrawRoundRect(100, 100, 700, 1305, 30, 30, cardPaint);
        }

        {
            // 处理曲绘
            using var originalSongCover = SKBitmap.Decode(cover);
            using var scaledSongCover = new SKBitmap(500, 500);
            originalSongCover.ScalePixels(scaledSongCover, SKFilterQuality.Medium);

            canvas.DrawBitmap(scaledSongCover, 200, 200);
        }

        {
            using var titlePaint = new SKPaint
            {
                Color = pref.Dark ? SKColors.White : SKColor.Parse("#333333"),
                Typeface = _fonts.TitilliumWeb_SemiBold,
                IsAntialias = true,
                TextSize = 53,
                TextAlign = SKTextAlign.Center
            };
            canvas.DrawText(ArcaeaImageUtils.ReplaceNotSupportedChar(record.Name),
                160, 770, titlePaint);
        }

        {
            var clearImg = _resources.GetClearTypeImage(record.ClearType);
            using var originalClearImg = SKBitmap.Decode(clearImg);

            if (originalClearImg.Height > 77)
            {
                using var scaledClearImg = new SKBitmap(600, 74);
                originalClearImg.ScalePixels(scaledClearImg, SKFilterQuality.Medium);
                canvas.DrawBitmap(scaledClearImg, 150, 790);
            }
            else
            {
                using var scaledClearImg = new SKBitmap(600, 53);
                originalClearImg.ScalePixels(scaledClearImg, SKFilterQuality.Medium);
                canvas.DrawBitmap(scaledClearImg, 150, 800);
            }
        }

        {
            using var textPaint = new SKPaint
            {
                Color = pref.Dark ? SKColors.White : SKColor.Parse("#333333"),
                TextSize = 85,
                IsAntialias = true,
                Typeface = _fonts.GeoSans,
                TextAlign = SKTextAlign.Center
            };
            canvas.DrawText(record.Score.ToDisplayScore(), 200, 944, textPaint);
        }


        if (record.Grade == ArcaeaGrade.EX)
        {
            var exImg = SKBitmap.Decode(_resources.GetGradeImage(ArcaeaGrade.EX));
            var scaledExImg = new SKBitmap(132, 111);
            exImg.ScalePixels(scaledExImg, SKFilterQuality.Medium);
            canvas.DrawBitmap(scaledExImg, 384, 980);
        }
        else
        {
            var gradeImg = SKBitmap.Decode(_resources.GetGradeImage(record.Grade));
            canvas.DrawBitmap(gradeImg, 335, 980);
        }

        {
            // Pure/Far/Lost 信息
            using var textPaint = new SKPaint
            {
                TextSize = 40,
                IsAntialias = true,
                Typeface = _fonts.GeoSans,
                Color = SKColors.White
            };
            // Pure
            if (!pref.Dark) textPaint.Color = SKColor.Parse("#6f3a5f");
            canvas.DrawText($"Pure / {record.PureCount} (+{record.ShinyPureCount})", 300, 1150, textPaint);

            // Far
            if (!pref.Dark) textPaint.Color = SKColor.Parse("#c19c00");
            canvas.DrawText($"Far / {record.FarCount}", 300, 1200, textPaint);

            // Lost
            if (!pref.Dark) textPaint.Color = SKColor.Parse("#bb2b43");
            canvas.DrawText($"Lost / {record.LostCount}", 300, 1250, textPaint);
        }

        {
            // 距离当前天数
            using var rectPaint = new SKPaint
            {
                Color = SKColor.Parse(pref.Dark ? "#333333" : "#dddddd"),
                IsAntialias = true
            };
            using var textPaint = new SKPaint
            {
                Color = SKColor.Parse(pref.Dark ? "#ffffff" : "#333333"),
                TextSize = 38,
                IsAntialias = true,
                Typeface = _fonts.TitilliumWeb_Regular,
                TextAlign = SKTextAlign.Center
            };

            var days = (int)(DateTime.UtcNow - record.PlayTime).TotalDays;

            canvas.DrawRoundRect(150, 1295, 600, 60, 10, 10, rectPaint);
            canvas.DrawText($"{days}d", 655, 1337, textPaint);
        }

        {
            // 难度条
            using var rectPaint = new SKPaint
            {
                Color = SKColor.Parse(pref.Dark ? colorInnerDark : colorDark),
                IsAntialias = true
            };
            using var textPaint = new SKPaint
            {
                Color = SKColor.Parse("#ffffff"),
                TextSize = 38,
                IsAntialias = true,
                Typeface = _fonts.TitilliumWeb_Regular
            };

            canvas.DrawRoundRect(150, 1295, 490, 60, 10, 10, rectPaint);
            canvas.DrawLimitedText(
                $"{record.Difficulty} {record.Rating.ToDisplayRating()} [{record.Rating:N1}]",
                332, 1337, textPaint, 334);

            // Border
            if (pref.Dark)
            {
                using var borderPaint = new SKPaint
                {
                    Color = SKColor.Parse(colorBorderDark),
                    IsAntialias = true,
                    IsStroke = true,
                    StrokeWidth = 3
                };
                canvas.DrawRoundRect(150, 1295, 490, 60, 10, 10, borderPaint);
            }
        }

        {
            // 获得 ptt
            using var rectPaint = new SKPaint
            {
                Color = SKColor.Parse(pref.Dark ? colorInnerLight : colorLight)
            };
            using var textPaint = new SKPaint
            {
                Color = SKColors.White,
                TextSize = 38,
                IsAntialias = true,
                Typeface = _fonts.TitilliumWeb_SemiBold
            };
            canvas.DrawRoundRect(150, 1295, 162, 60, 10, 10, rectPaint);
            canvas.DrawText($"{record.Potential:0.0000}", 166, 1337, textPaint);

            // Border
            if (pref.Dark)
            {
                using var borderPaint = new SKPaint
                {
                    Color = SKColor.Parse(colorBorderLight),
                    IsAntialias = true,
                    IsStroke = true,
                    StrokeWidth = 3
                };
                canvas.DrawRoundRect(150, 1295, 162, 60, 10, 10, borderPaint);
            }
        }

        {
            using var textPaint = new SKPaint
            {
                Color = SKColors.White,
                TextSize = 38,
                IsAntialias = true,
                Typeface = _fonts.TitilliumWeb_Regular,
                TextAlign = SKTextAlign.Center
            };
            canvas.DrawText("Generated by YukiChan", 100, 1477, textPaint);
        }

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 70);
        return data.ToArray();
    }

    public async Task<byte[]> GetSingleV1Background(ArcaeaRecord record)
    {
        var dir = $"{ArcaeaPlugin.CacheDirectory}/single-dynamic-bg-v1";
        Directory.CreateDirectory(dir);

        var path = $"{dir}/{record.SongId}{
            (record.JacketOverride ? $"-{(int)record.Difficulty}" : "")}.jpg";

        if (File.Exists(path))
            return await File.ReadAllBytesAsync(path);

        var coverBitmap = SKBitmap.Decode(
            await _cacheManager.GetSongCoverAsync(
                record.SongId, record.JacketOverride, record.Difficulty))!;
        using var scaledCoverBitmap = new SKBitmap(1520, 1520);
        coverBitmap.ScalePixels(scaledCoverBitmap, SKFilterQuality.Low);
        coverBitmap.Dispose();

        var imageInfo = new SKImageInfo(900, 1520);
        using var surface = SKSurface.Create(imageInfo);
        using var canvas = surface.Canvas;

        using var paint = new SKPaint
        {
            ImageFilter = SKImageFilter.CreateBlur(15, 15)
        };
        canvas.DrawBitmap(scaledCoverBitmap, -310, 0, paint);

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Jpeg, 70);

        var result = data.ToArray();

        await File.WriteAllBytesAsync(path, result);

        return result;
    }
}