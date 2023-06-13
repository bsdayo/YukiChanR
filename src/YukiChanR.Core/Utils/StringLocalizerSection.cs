using Microsoft.Extensions.Localization;

namespace YukiChanR.Core.Utils;

public sealed class StringLocalizerSection<T> : IStringLocalizer<T>
{
    private readonly IStringLocalizer<T> _localizer;
    public string Section { get; set; }

    internal StringLocalizerSection(IStringLocalizer<T> localizer, string section)
    {
        _localizer = localizer;
        Section = section;
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) =>
        _localizer.GetAllStrings(includeParentCultures);

    public LocalizedString this[string name] =>
        _localizer[$"{Section}:{name}"];

    public LocalizedString this[string name, params object[] arguments] =>
        _localizer[$"{Section}:{name}", arguments];
}