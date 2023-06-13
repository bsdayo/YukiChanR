namespace YukiChanR.Plugins.Arcaea;

public sealed class ArcaeaPluginOptions
{
    public string UaaApiUrl { get; set; } = "";

    public string UaaToken { get; set; } = "";

    public TimeSpan UaaTimeout { get; set; } = TimeSpan.FromMinutes(1);

    public TimeSpan GuessTime { get; set; } = TimeSpan.FromSeconds(30);
}