using Microsoft.Extensions.Localization;

namespace YukiChanR.Core.Utils;

public static class StringLocalizerExtensions
{
    public static StringLocalizerSection<T> GetSection<T>(this IStringLocalizer<T> localizer, string name)
    {
        if (localizer is StringLocalizerSection<T> section)
            name = $"{section.Section}:{name}";
        return new StringLocalizerSection<T>(localizer, name);
    }
}