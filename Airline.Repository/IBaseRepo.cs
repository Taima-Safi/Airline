
using System.Linq.Expressions;

namespace Airline.Repository;

public interface IBaseRepo<T> where T : class
{
    Task AddAsync(T item);
    Task<bool> CheckIfExistAsync(Expression<Func<T, bool>> expression);
    Task<List<T>> GetAllAsync(Expression<Func<T, bool>> expression);
    Task<T> GetByAsync(Expression<Func<T, bool>> expression);
}
