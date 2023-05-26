using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace YukiChanR.Core;

public abstract class YukiDbContext : DbContext
{
    private readonly string _connStr;

    protected YukiDbContext(IOptions<YukiOptions> options)
    {
        _connStr = options.Value.Database.GetConnectionString();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseNpgsql(_connStr);
    }
}