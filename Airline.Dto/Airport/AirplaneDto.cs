using Airline.Dto.Seat;

namespace Airline.Dto.Airport;

public class AirplaneDto
{
    public long Id { get; set; }
    public string Title { get; set; }
    public List<SeatDto> Seats { get; set; } = [];
}
