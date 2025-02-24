using Airline.Dto.Flight;
using Airline.Dto.Seat;
using Airline.Dto.User;
using Airline.Shared.Enum;

namespace Airline.Dto.Book;

public class BookDto
{
    public double Price { get; set; }
    public DateTime Date { get; set; }
    public BookStatus BookStatus { get; set; }
    public SeatDto Seat { get; set; }
    public UserDto User { get; set; }
    public FlightDto Flight { get; set; }
}
