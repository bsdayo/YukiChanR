using System.Reflection;

namespace YukiChanR.Plugins.Monitor;

public sealed class BuildInfo
{
    private static string GetMetadata(string key)
    {
        return typeof(BuildInfo).Assembly
            .GetCustomAttributes<AssemblyMetadataAttribute>()
            .First(attr => attr.Key == key)
            .Value!;
    }

    public static readonly string RefName = GetMetadata("BuildRefName");

    public static readonly string FullCommitHash = GetMetadata("BuildCommitHash");

    public static readonly string ShortCommitHash = FullCommitHash[..7];

    public static readonly string[] ExtraTags = GetMetadata("BuildExtraTags")
        .Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
}