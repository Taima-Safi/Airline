using Airline.Dto.Flight;

namespace Airline.Service.Flight;

public interface IFlightService
{
    Task AddFlightAsync(FlightFormDto dto);
    Task<List<FlightDto>> GetAllFlightsAsync(string code, string arrivalCityTitle, string departureCityTitle, DateTime? departureDate, DateTime? arrivalDate);
    Task<FlightDto> GetFlightByIdAsync(long id);
    Task RemoveFlightAsync(long flightId);
    Task UpdateFlightAsync(long flightId, FlightFormDto dto);
}
