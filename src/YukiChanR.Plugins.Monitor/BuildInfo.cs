using System.Reflection;

namespace YukiChanR.Plugins.Monitor;

public sealed class BuildInfo
{
    private static string[] _info = typeof(BuildInfo)
        .Assembly
        .GetCustomAttributes<AssemblyMetadataAttribute>()
        .First(attr => attr.Key == "BuildInfo")
        .Value!
        .Split(';');

    public static readonly string Branch = _info[0];

    public static readonly string FullCommitHash = _info[1];

    public static readonly string ShortCommitHash = _info[1][..7];
}