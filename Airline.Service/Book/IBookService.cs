using Airline.Dto.Book;

namespace Airline.Service.Book;

public interface IBookService
{
    Task AddBookAsync(BookFormDto dto);
}
