using Airline.Shared.Enum;

namespace Airline.Database.Model;

public class FlightClassPriceModel : BaseModel
{
    public double Price { get; set; }
    public bool IsActive { get; set; }
    public SeatClass Type { get; set; }

    public long FlightId { get; set; }
    public FlightModel Flight { get; set; }
}
