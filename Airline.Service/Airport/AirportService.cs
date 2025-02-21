using Airline.Database.Model;
using Airline.Repository;

namespace Airline.Service.Airport;

public class AirportService : IAirportService
{
    private readonly IBaseRepo<AirportModel> airportBaseRepo;

    public AirportService(IBaseRepo<AirportModel> airportBaseRepo)
    {
        this.airportBaseRepo = airportBaseRepo;
    }
}
