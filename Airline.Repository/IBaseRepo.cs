
namespace Airline.Repository;

public interface IBaseRepo<T> where T : class
{
    Task AddAsync(T item);
}
