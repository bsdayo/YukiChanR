using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using YukiChanR.Core;
using YukiChanR.Plugins.Arcaea.Entities;

namespace YukiChanR.Plugins.Arcaea;

public sealed class ArcaeaDbContext : YukiDbContext
{
    public ArcaeaDbContext(IOptions<YukiOptions> options) : base(options)
    {
    }

    public DbSet<ArcaeaDatabaseUser> Users => Set<ArcaeaDatabaseUser>();

    public DbSet<ArcaeaUserPreferences> Preferences => Set<ArcaeaUserPreferences>();
}