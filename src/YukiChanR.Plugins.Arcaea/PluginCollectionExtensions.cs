using Flandre.Framework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using YukiChanR.Plugins.Arcaea.ImageGen;

namespace YukiChanR.Plugins.Arcaea;

public static class PluginCollectionExtensions
{
    private static void AddArcaeaServices(IPluginCollection plugins)
    {
        plugins.Services.AddLocalization();
        plugins.Services.TryAddSingleton<ArcaeaResourceManager>();
        plugins.Services.TryAddSingleton<ArcaeaCacheManager>();
        plugins.Services.TryAddSingleton<ArcaeaImageGenerator>();
        plugins.Services.TryAddSingleton<UaaService>();
        plugins.Services.AddDbContext<ArcaeaSongDbContext>();
        plugins.Services.AddDbContext<ArcaeaDbContext>();
    }

    public static void AddArcaea(this IPluginCollection plugins)
    {
        var config = plugins.Configuration.GetSection("Plugins:Arcaea");
        AddArcaeaServices(plugins);
        plugins.Add<ArcaeaPlugin, ArcaeaPluginOptions>(config);
    }

    public static void AddArcaea(this IPluginCollection plugins, Action<ArcaeaPluginOptions> action)
    {
        AddArcaeaServices(plugins);
        plugins.Add<ArcaeaPlugin, ArcaeaPluginOptions>(action);
    }
}