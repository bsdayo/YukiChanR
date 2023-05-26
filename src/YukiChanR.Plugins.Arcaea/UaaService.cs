using Microsoft.Extensions.Options;
using UnofficialArcaeaAPI.Lib;

namespace YukiChanR.Plugins.Arcaea;

/// <summary>
/// Registered as singleton.
/// </summary>
public sealed class UaaService
{
    public UaaClient UaaClient { get; }

    public UaaService(IOptions<ArcaeaPluginOptions> options)
    {
        UaaClient = new UaaClient(new UaaClientOptions
        {
            ApiUrl = options.Value.UaaApiUrl,
            Token = options.Value.UaaToken,
            Timeout = options.Value.UaaTimeout
        });
    }
}