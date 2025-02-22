using Airline.Shared.Enum;

namespace Airline.Dto.Seat;

public class SeatFormDto
{
    public string Code { get; set; }
    public SeatClass Type { get; set; }
    public long AirplaneId { get; set; }

}
