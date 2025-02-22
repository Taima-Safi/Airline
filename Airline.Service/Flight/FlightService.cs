using Airline.Database.Model;
using Airline.Dto.Airport;
using Airline.Dto.Flight;
using Airline.Repository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Airline.Service.Flight;

public class FlightService : IFlightService
{
    private readonly IBaseRepo<FlightModel> flightBaseRepo;
    private readonly IBaseRepo<AirportModel> airportBaseRepo;


    public FlightService(IBaseRepo<FlightModel> flightBaseRepo, IBaseRepo<AirportModel> airportBaseRepo)
    {
        this.flightBaseRepo = flightBaseRepo;
        this.airportBaseRepo = airportBaseRepo;
    }

    public async Task AddFlightAsync(FlightFormDto dto)
    {

        if (!await airportBaseRepo.CheckIfExistAsync(a => a.Id == dto.ArrivalAirportId && a.IsValid))
            throw new ArgumentException("Arrival Airport not found.");

        if (!await airportBaseRepo.CheckIfExistAsync(a => a.Id == dto.DepartureAirportId && a.IsValid))
            throw new ArgumentException("Departure Airport not found.");

        await flightBaseRepo.AddAsync(new FlightModel
        {
            Code = dto.Code,
            ArrivalDate = dto.ArrivalDate,
            DepartureDate = dto.DepartureDate,
            ArrivalAirportId = dto.ArrivalAirportId,
            DepartureAirportId = dto.DepartureAirportId,
        });
    }
    public async Task<List<FlightDto>> GetAllFlightsAsync(string code, string arrivalCityTitle, string departureCityTitle, DateTime? departureDate, DateTime? arrivalDate)
    {
        Expression<Func<FlightModel, bool>> expression = f => (string.IsNullOrEmpty(code) || f.Code.Contains(code))
        && (string.IsNullOrEmpty(departureCityTitle) || f.DepartureAirport.City.Title.Contains(departureCityTitle))
        && (string.IsNullOrEmpty(arrivalCityTitle) || f.ArrivalAirport.City.Title.Contains(arrivalCityTitle))
        && (!departureDate.HasValue || f.DepartureDate == departureDate)
        && (!arrivalDate.HasValue || f.ArrivalDate == arrivalDate)
        && f.IsValid;

        var flights = await flightBaseRepo.GetAllAsync(expression, x => x.Include(x => x.ArrivalAirport).ThenInclude(c => c.City)
        , x => x.Include(x => x.DepartureAirport).ThenInclude(c => c.City));

        return flights.Select(f => new FlightDto
        {
            Id = f.Id,
            Code = f.Code,
            ArrivalDate = f.ArrivalDate,
            DepartureDate = f.DepartureDate,
            ArrivalAirport = new AirportDto
            {
                Id = f.ArrivalAirport.Id,
                Title = f.ArrivalAirport.Title,
                CityId = f.ArrivalAirport.City.Id,
                CityTitle = f.ArrivalAirport.City.Title
            },
            DepartureAirport = new AirportDto
            {
                Id = f.DepartureAirport.Id,
                Title = f.DepartureAirport.Title,
                CityId = f.DepartureAirport.City.Id,
                CityTitle = f.DepartureAirport.City.Title
            }
        }).ToList();
    }
    public async Task<FlightDto> GetFlightByIdAsync(long id)
    {
        Expression<Func<FlightModel, bool>> expression = f => f.Id == id && f.IsValid;
        if (!await flightBaseRepo.CheckIfExistAsync(expression))
            throw new ArgumentException("Flight not found");

        var flight = await flightBaseRepo.GetByAsync(expression, x => x.Include(x => x.ArrivalAirport).ThenInclude(c => c.City)
        , x => x.Include(x => x.DepartureAirport).ThenInclude(c => c.City));

        return new FlightDto
        {
            Id = flight.Id,
            Code = flight.Code,
            ArrivalDate = flight.ArrivalDate,
            DepartureDate = flight.DepartureDate,
            ArrivalAirport = new AirportDto
            {
                Id = flight.ArrivalAirport.Id,
                Title = flight.ArrivalAirport.Title,
                CityId = flight.ArrivalAirport.City.Id,
                CityTitle = flight.ArrivalAirport.City.Title
            },
            DepartureAirport = new AirportDto
            {
                Id = flight.DepartureAirport.Id,
                Title = flight.DepartureAirport.Title,
                CityId = flight.DepartureAirport.City.Id,
                CityTitle = flight.DepartureAirport.City.Title
            }
        };
    }
    public async Task UpdateFlightAsync(long flightId, FlightFormDto dto)
    {
        var model = await flightBaseRepo.GetByAsync(x => x.Id == flightId && x.IsValid) ??
            throw new ArgumentException("Flight not found.");

        if (!await airportBaseRepo.CheckIfExistAsync(a => a.Id == dto.ArrivalAirportId && a.IsValid))
            throw new ArgumentException("Arrival Airport not found.");

        if (!await airportBaseRepo.CheckIfExistAsync(a => a.Id == dto.DepartureAirportId && a.IsValid))
            throw new ArgumentException("Departure Airport not found.");

        model.Code = dto.Code;
        model.ArrivalDate = dto.ArrivalDate;
        model.DepartureDate = dto.DepartureDate;
        model.ArrivalAirportId = dto.ArrivalAirportId;
        model.DepartureAirportId = dto.DepartureAirportId;

        await flightBaseRepo.SaveChangesAsync();

        //await flightBaseRepo.UpdateAsync(
        //f => f.Id == flightId && f.IsValid, // Condition
        //setters => setters
        //    .SetProperty(x => x.Code, dto.Code)
        //    .SetProperty(x => x.ArrivalDate, dto.ArrivalDate)
        //    .SetProperty(x => x.DepartureDate, dto.DepartureDate)
        //    .SetProperty(x => x.ArrivalAirportId, dto.ArrivalAirportId)
        //    .SetProperty(x => x.DepartureAirportId, dto.DepartureAirportId));
    }
    public async Task RemoveFlightAsync(long flightId)
    {
        Expression<Func<FlightModel, bool>> expression = x => x.Id == flightId && x.IsValid;

        if (!await flightBaseRepo.CheckIfExistAsync(expression))
            throw new ArgumentException("Flight not found.");

        await flightBaseRepo.RemoveAsync(expression,
            setters => setters.SetProperty(x => x.IsValid, false));
    }
}
