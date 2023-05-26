namespace YukiChanR.Core;

public static class YukiDirectories
{
    public const string Configs = "configs";

    public const string Data = "data";

    public const string Logs = "logs";

    public const string Cache = "cache";

    public static readonly string Temp = Path.Combine(Path.GetTempPath(), "YukiChanR");
}