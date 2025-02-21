using Airline.Database.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Airline.Repository;

public class BaseRepo<T> : IBaseRepo<T> where T : class
{
    protected readonly AirlineDbContext context;
    protected readonly DbSet<T> Entity;

    public BaseRepo(AirlineDbContext context)
    {
        this.context = context;
        this.Entity = context.Set<T>();
    }

    public async Task AddAsync(T item)
    {
        await Entity.AddAsync(item);
        await context.SaveChangesAsync();
    }

    public async Task<bool> CheckIfExist(Expression<Func<T, bool>> expression)
        => await Entity.Where(expression).AnyAsync();
}
