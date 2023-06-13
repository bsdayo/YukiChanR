using Microsoft.Extensions.Localization;

namespace YukiChanR.Core.Utils;

public static class StringLocalizerExtensions
{
    public static string GetReply(this IStringLocalizer localizer, string key, params object[] args)
    {
        return localizer[$"{key}:Reply", args];
    }
}