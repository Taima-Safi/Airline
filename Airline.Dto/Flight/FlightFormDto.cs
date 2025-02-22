namespace Airline.Dto.Flight;

public class FlightFormDto
{
    public string Code { get; set; }

    public DateTime ArrivalDate { get; set; }
    public DateTime DepartureDate { get; set; }

    public long ArrivalAirportId { get; set; }
    public long DepartureAirportId { get; set; }
}