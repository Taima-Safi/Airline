using Airline.Dto.Airport;

namespace Airline.Dto.Flight;

public class FlightDto
{
    public long Id { get; set; }
    public string Code { get; set; }

    public DateTime ArrivalDate { get; set; }
    public DateTime DepartureDate { get; set; }

    public AirplaneDto Airplane { get; set; }
    public AirportDto ArrivalAirport { get; set; }
    public AirportDto DepartureAirport { get; set; }
}
