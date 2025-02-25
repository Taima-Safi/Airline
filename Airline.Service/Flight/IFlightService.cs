using Airline.Dto.Flight;
using Airline.Shared.Enum;

namespace Airline.Service.Flight;

public interface IFlightService
{
    Task AddFlightAsync(FlightFormDto dto);
    Task<List<FlightDto>> GetAllFlightsAsync(string code, string airplaneTitle, string arrivalCityTitle, string departureCityTitle, DateTime? departureDate, DateTime? arrivalDate);
    Task<FlightDto> GetFlightByIdAsync(long id, bool justActivePrice);
    Task RemoveFlightAsync(long flightId);
    Task UpdateFlightAsync(long flightId, FlightFormDto dto);
    Task UpdateFlightClassPriceAsync(long flightId, SeatClass type, double price);
}
