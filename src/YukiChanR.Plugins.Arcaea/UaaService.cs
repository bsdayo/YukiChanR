using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using UnofficialArcaeaAPI.Lib;

namespace YukiChanR.Plugins.Arcaea;

/// <summary>
/// Registered as singleton.
/// </summary>
public sealed class UaaService
{
    public UaaClient UaaClient { get; }

    private readonly IStringLocalizer<UaaService> _localizer;

    public UaaService(IOptions<ArcaeaPluginOptions> options, IStringLocalizer<UaaService> localizer)
    {
        UaaClient = new UaaClient(new UaaClientOptions
        {
            ApiUrl = options.Value.UaaApiUrl,
            Token = options.Value.UaaToken,
            Timeout = options.Value.UaaTimeout
        });
        _localizer = localizer;
    }

    public string GetExceptionReply(UaaRequestException exception)
    {
        var reply = _localizer[$"ErrorStatuses:{exception.Status}"];
        return reply.ResourceNotFound ? $"Error {exception.Status}: {exception.Message}" : reply;
    }
}