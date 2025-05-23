﻿using Airline.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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
    public async Task AddListAsync(List<T> items)
    {
        await Entity.AddRangeAsync(items);
        await context.SaveChangesAsync();
    }

    public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> expression, params Func<IQueryable<T>, IQueryable<T>>[] includes)
    {
        var query = Entity.AsQueryable();

        if (includes != null)
            foreach (var include in includes)
                query = include(query);
        //query = includes.Aggregate(query, (current, include) => include(current));

        return await query.Where(expression).ToListAsync();
    }

    public async Task<T> GetByAsync(Expression<Func<T, bool>> expression, params Func<IQueryable<T>, IQueryable<T>>[] includes)
    {
        var query = Entity.Where(expression);

        if (includes != null)
            foreach (var include in includes)
                query = include(query);

        return await query.FirstOrDefaultAsync();
    }


    public async Task UpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setProperties)
    {
        await Entity.Where(predicate).ExecuteUpdateAsync(setProperties);
        await context.SaveChangesAsync();
    }
    public async Task RemoveAsync(Expression<Func<T, bool>> predicate, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setProperties)
    {
        await Entity.Where(predicate).ExecuteUpdateAsync(setProperties);
        await context.SaveChangesAsync();
    }
    public async Task HardRemoveAsync(Expression<Func<T, bool>> predicate)
    {
        await Entity.Where(predicate).ExecuteDeleteAsync();
        await context.SaveChangesAsync();
    }

    public async Task<bool> CheckIfExistAsync(Expression<Func<T, bool>> expression)
        => await Entity.AnyAsync(expression);
    public async Task SaveChangesAsync() => await context.SaveChangesAsync();
}
