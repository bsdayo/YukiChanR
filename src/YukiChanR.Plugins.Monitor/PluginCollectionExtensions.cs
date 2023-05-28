using Flandre.Framework;

namespace YukiChanR.Plugins.Monitor;

public static class PluginCollectionExtensions
{
    public static void AddMonitor(this IPluginCollection plugins)
    {
        plugins.Add<MonitorPlugin>();
    }
}