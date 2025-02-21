
namespace Airline.Service.Country;

public interface ICountryService
{
    Task AddCityAsync(string title, long countryId);
    Task AddCountryAsync(string title);
}
