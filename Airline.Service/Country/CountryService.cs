using Airline.Database.Model;
using Airline.Repository;

namespace Airline.Service.Country;

public class CountryService : ICountryService
{
    private readonly IBaseRepo<CountryModel> countryBaseRepo;
    private readonly IBaseRepo<CityModel> cityBaseRepo;

    public CountryService(IBaseRepo<CityModel> cityBaseRepo, IBaseRepo<CountryModel> countryBaseRepo)
    {
        this.cityBaseRepo = cityBaseRepo;
        this.countryBaseRepo = countryBaseRepo;
    }

    public async Task AddCountryAsync(string title)
    => await countryBaseRepo.AddAsync(new CountryModel { Title = title });
}
