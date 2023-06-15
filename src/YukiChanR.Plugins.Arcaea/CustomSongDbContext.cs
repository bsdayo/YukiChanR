using Microsoft.EntityFrameworkCore;

namespace YukiChanR.Plugins.Arcaea;

public sealed class CustomSongDbContext : ArcaeaSongDbContext
{
    internal static string CustomSongDbPath { get; } = Path.Combine(ArcaeaPlugin.DataDirectory, "customsong.db");

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"DataSource={CustomSongDbPath}");
}