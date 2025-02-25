
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Airline.Repository;

public interface IBaseRepo<T> where T : class
{
    Task AddAsync(T item);
    Task AddListAsync(List<T> items);
    Task<bool> CheckIfExistAsync(Expression<Func<T, bool>> expression);
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>> expression, params Func<IQueryable<T>, IQueryable<T>>[] includes);
    Task<T> GetByAsync(Expression<Func<T, bool>> expression, params Func<IQueryable<T>, IQueryable<T>>[] includes);
    Task RemoveAsync(Expression<Func<T, bool>> predicate, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setProperties);
    Task SaveChangesAsync();

    // Task UpdateAsync(T entity);
    Task UpdateAsync(Expression<Func<T, bool>> predicate, Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setProperties);
}
