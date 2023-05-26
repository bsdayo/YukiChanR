using UnofficialArcaeaAPI.Lib.Models;

namespace YukiChanR.Plugins.Arcaea.Models;

public sealed class ArcaeaUser
{
    public string Name { get; set; } = "";

    public string Code { get; set; } = "";

    public double Potential { get; set; }

    public DateTime JoinTime { get; set; }

    public int PartnerId { get; set; }

    public bool IsPartnerAwakened { get; set; }

    public static ArcaeaUser FromUaa(UaaAccountInfo info)
    {
        return new ArcaeaUser
        {
            Name = info.Name,
            Code = info.Code,
            Potential = (double)info.Rating / 100,
            JoinTime = DateTimeOffset.FromUnixTimeMilliseconds(info.JoinDate).UtcDateTime,
            PartnerId = info.Character,
            IsPartnerAwakened = info.IsCharUncapped
        };
    }
}