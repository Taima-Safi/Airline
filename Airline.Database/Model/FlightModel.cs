namespace Airline.Database.Model;

public class FlightModel : BaseModel
{
    public string Title { get; set; }

    public DateTime ArrivalAirportDate { get; set; }
    public DateTime DepartureAirportDate { get; set; }

    public long ArrivalAirportId { get; set; }
    public AirportModel ArrivalAirport { get; set; }

    public long DepartureAirportId { get; set; }
    public AirportModel DepartureAirport { get; set; }
}
