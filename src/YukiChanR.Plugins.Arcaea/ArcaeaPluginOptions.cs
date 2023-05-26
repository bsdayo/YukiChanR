namespace YukiChanR.Plugins.Arcaea;

public sealed class ArcaeaPluginOptions
{
    public string UaaApiUrl { get; set; } = "";

    public string UaaToken { get; set; } = "";

    public TimeSpan UaaTimeout { get; set; }

    public bool EnableGuess { get; set; } = true;
}