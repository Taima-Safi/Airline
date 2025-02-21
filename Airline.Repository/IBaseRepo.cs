
using System.Linq.Expressions;

namespace Airline.Repository;

public interface IBaseRepo<T> where T : class
{
    Task AddAsync(T item);
    Task<bool> CheckIfExist(Expression<Func<T, bool>> expression);
}
