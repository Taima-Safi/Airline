using Airline.Database.Context;
using Microsoft.EntityFrameworkCore;

namespace Airline.Repository;

public class BaseRepo<T> : IBaseRepo<T> where T : class
{
    protected readonly AirlineDbContext context;
    protected readonly DbSet<T> Entity;

    public BaseRepo(DbSet<T> Entity, AirlineDbContext context)
    {
        this.Entity = Entity;
        this.context = context;
    }

    public async Task AddAsync(T item)
    {
        await Entity.AddAsync(item);
        await context.SaveChangesAsync();
    }
}
