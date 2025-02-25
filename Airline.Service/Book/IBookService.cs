using Airline.Dto.Book;

namespace Airline.Service.Book;

public interface IBookService
{
    Task<string> AddBookAsync(BookFormDto dto);
    Task<List<BookDto>> GetAllFlightBooksAsync(long flightId, string userBookCode, bool? isConfirmed);
    Task RemoveBookAsync(long bookId);
    Task SendBookingConfirmation(string toEmail, string status);
    Task UpdateBookStatusAsync(string userBookCode, bool isConfirmed);
}
