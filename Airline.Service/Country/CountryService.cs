using Airline.Database.Model;
using Airline.Repository;

namespace Airline.Service.Country;

public class CountryService : ICountryService
{
    private readonly IBaseRepo<CityModel> cityBaseRepo;
    private readonly IBaseRepo<CountryModel> countryBaseRepo;

    public CountryService(IBaseRepo<CityModel> cityBaseRepo, IBaseRepo<CountryModel> countryBaseRepo)
    {
        this.cityBaseRepo = cityBaseRepo;
        this.countryBaseRepo = countryBaseRepo;
    }

    public async Task AddCountryAsync(string title)
    => await countryBaseRepo.AddAsync(new CountryModel { Title = title });

    public async Task AddCityAsync(string title, long countryId)
    {
        if (!await countryBaseRepo.CheckIfExistAsync(c => c.Id == countryId && c.IsValid))
            throw new ArgumentException("Country does not exist or is not valid.");

        await cityBaseRepo.AddAsync(new CityModel { Title = title, CountryId = countryId });
    }
}
