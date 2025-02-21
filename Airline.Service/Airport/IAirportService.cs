
namespace Airline.Service.Airport;

public interface IAirportService
{
    Task AddAirportAsync(string title, long cityId);
}
