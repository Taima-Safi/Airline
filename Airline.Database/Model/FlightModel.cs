namespace Airline.Database.Model;

public class FlightModel : BaseModel
{
    public string Code { get; set; }

    public DateTime ArrivalDate { get; set; }
    public DateTime DepartureDate { get; set; }

    public long AirplaneId { get; set; }
    public AirplaneModel Airplane { get; set; }

    public long ArrivalAirportId { get; set; }
    public AirportModel ArrivalAirport { get; set; }

    public long DepartureAirportId { get; set; }
    public AirportModel DepartureAirport { get; set; }

    public ICollection<FlightClassPriceModel> FlightClassPrices { get; set; }
}
