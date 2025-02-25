using Airline.Shared.Enum;

namespace Airline.Dto.Flight;

public class FlightClassPriceDto
{
    public double Price { get; set; }
    public bool IsActive { get; set; }
    public SeatClass Type { get; set; }
}
