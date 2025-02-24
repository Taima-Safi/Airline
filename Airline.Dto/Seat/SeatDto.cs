using Airline.Dto.Book;
using Airline.Shared.Enum;

namespace Airline.Dto.Seat;

public class SeatDto
{
    public long Id { get; set; }
    public string Code { get; set; }
    public SeatClass Type { get; set; }

    public long AirplaneId { get; set; }
    public string AirplaneTitle { get; set; }
    public List<BookDto> Books { get; set; } = [];
}
