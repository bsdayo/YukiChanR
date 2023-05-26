using Microsoft.Extensions.Logging;
using UnofficialArcaeaAPI.Lib.Models;

namespace YukiChanR.Plugins.Arcaea;

/// <summary>
/// Registered as singleton.
/// </summary>
public sealed class ArcaeaCacheManager
{
    private readonly UaaService _uaaService;
    private readonly ILogger<ArcaeaCacheManager> _logger;

    private static readonly string CoverCacheDirectory = Path.Combine(ArcaeaPlugin.CacheDirectory, "covers");
    private static readonly string CharacterCacheDirectory = Path.Combine(ArcaeaPlugin.CacheDirectory, "characters");
    private static readonly string PreviewCacheDirectory = Path.Combine(ArcaeaPlugin.CacheDirectory, "preview");

    public ArcaeaCacheManager(UaaService uaaService, ILogger<ArcaeaCacheManager> logger)
    {
        _uaaService = uaaService;
        _logger = logger;
    }

    private async Task<byte[]> DownloadSongCoverAsync(string songId, ArcaeaDifficulty difficulty, string savePath)
    {
        Directory.CreateDirectory(CoverCacheDirectory);
        var cover = await _uaaService.UaaClient.Assets.GetSongAsync(songId, UaaSongQueryType.SongId, difficulty);
        await File.WriteAllBytesAsync(savePath, cover);
        return cover;
    }

    private async Task<byte[]> DownloadCharacterImageAsync(int characterId, bool awakened, string savePath)
    {
        Directory.CreateDirectory(CharacterCacheDirectory);
        var image = await _uaaService.UaaClient.Assets.GetCharAsync(characterId, awakened);
        await File.WriteAllBytesAsync(savePath, image);
        return image;
    }

    private async Task<byte[]> DownloadPreviewImageAsync(string songId, ArcaeaDifficulty difficulty, string savePath)
    {
        Directory.CreateDirectory(PreviewCacheDirectory);
        var image = await _uaaService.UaaClient.Assets.GetPreviewAsync(songId, UaaSongQueryType.SongId, difficulty);
        await File.WriteAllBytesAsync(savePath, image);
        return image;
    }

    public Task<byte[]> GetSongCoverAsync(string songId, bool jacketOverride = false,
        ArcaeaDifficulty difficulty = ArcaeaDifficulty.Future,
        bool nya = false)
    {
        var diff = jacketOverride || difficulty == ArcaeaDifficulty.Beyond
            ? difficulty
            : ArcaeaDifficulty.Future;
        var path = Path.Join(CoverCacheDirectory, $"{songId}-{(int)diff}.jpg");
        return File.Exists(path)
            ? File.ReadAllBytesAsync(path)
            : DownloadSongCoverAsync(songId, diff, path);
    }

    public Task<byte[]> GetCharacterImageAsync(int characterId, bool awakened)
    {
        var path = Path.Join(CharacterCacheDirectory, $"{characterId}{(awakened ? "-awakened" : "")}.jpg");
        return File.Exists(path)
            ? File.ReadAllBytesAsync(path)
            : DownloadCharacterImageAsync(characterId, awakened, path);
    }

    public Task<byte[]> GetPreviewImageAsync(string songId, ArcaeaDifficulty difficulty)
    {
        var path = Path.Join(PreviewCacheDirectory, $"{songId}-{(int)difficulty}.jpg");
        return File.Exists(path)
            ? File.ReadAllBytesAsync(path)
            : DownloadPreviewImageAsync(songId, difficulty, path);
    }
}