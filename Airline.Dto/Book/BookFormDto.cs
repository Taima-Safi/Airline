namespace Airline.Dto.Book;

public class BookFormDto
{
    public DateTime Date { get; set; }

    public long SeatId { get; set; }
    public long UserId { get; set; }
    public long FlightId { get; set; }
}
