﻿using Flandre.Framework.Common;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YukiChanR.Core;
using YukiChanR.Plugins.Arcaea.ImageGen;

namespace YukiChanR.Plugins.Arcaea;

public sealed partial class ArcaeaPlugin : Plugin
{
    private readonly ArcaeaPluginOptions _options;
    private readonly UaaService _uaaService;
    private readonly ArcaeaImageGenerator _imageGen;
    private readonly ArcaeaCacheManager _cacheManager;
    private readonly ArcaeaDbContext _database;
    private readonly ArcaeaSongDbContext _songDb;
    private readonly IStringLocalizer<ArcaeaPlugin> _localizer;
    private readonly ILogger<ArcaeaPlugin> _logger;

    public ArcaeaPlugin(
        IOptions<ArcaeaPluginOptions> options,
        UaaService uaaService,
        ArcaeaImageGenerator imageGen,
        ArcaeaCacheManager cacheManager,
        ArcaeaDbContext database,
        ArcaeaSongDbContext songDb,
        IStringLocalizer<ArcaeaPlugin> localizer,
        ILogger<ArcaeaPlugin> logger)
    {
        _options = options.Value;
        _uaaService = uaaService;
        _imageGen = imageGen;
        _cacheManager = cacheManager;
        _database = database;
        _songDb = songDb;
        _localizer = localizer;
        _logger = logger;
    }

    internal static readonly string CacheDirectory = Path.Join(YukiDirectories.Cache, "arcaea");

    internal static readonly string TempDirectory = Path.Combine(YukiDirectories.Temp, "arcaea");

    public override async Task OnLoadingAsync()
    {
        Directory.CreateDirectory(TempDirectory);

        var songDb = new ArcaeaResourceManager().GetData("Databases.arcsong.db");
        await File.WriteAllBytesAsync(ArcaeaSongDbContext.ExtractedPath, songDb);
    }
}