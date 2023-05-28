using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace YukiChanR.Core.Utils;

public static class DbContextExtensions
{
    public static async Task<T> FirstOrAddAsync<T>(this DbSet<T> set,
        Expression<Func<T, bool>> predicate) where T : class, new()
    {
        var entity = await set.FirstOrDefaultAsync(predicate);

        if (entity is null)
        {
            entity = new T();
            set.Entry(entity).State = EntityState.Added;
        }

        return entity;
    }

    public static async Task<T> FirstOrAddAsync<T>(this DbSet<T> set,
        Expression<Func<T, bool>> predicate, Func<T> entityFactory) where T : class
    {
        ArgumentNullException.ThrowIfNull(entityFactory);

        var entity = await set.FirstOrDefaultAsync(predicate);

        if (entity is null)
        {
            entity = entityFactory();
            set.Entry(entity).State = EntityState.Added;
        }

        return entity;
    }
}