using Airline.Database.Model;
using Airline.Repository;

namespace Airline.Service.Airport;

public class AirportService : IAirportService
{
    private readonly IBaseRepo<CityModel> cityBaseRepo;
    private readonly IBaseRepo<AirportModel> airportBaseRepo;

    public AirportService(IBaseRepo<AirportModel> airportBaseRepo, IBaseRepo<CityModel> cityBaseRepo)
    {
        this.airportBaseRepo = airportBaseRepo;
        this.cityBaseRepo = cityBaseRepo;
    }

    public async Task AddAirportAsync(string title, long cityId)
    {
        if (!await cityBaseRepo.CheckIfExist(c => c.Id == cityId && c.IsValid))
            throw new ArgumentException("City does not exist or is not valid.");

        await airportBaseRepo.AddAsync(new AirportModel { Title = title, CityId = cityId });
    }
}
