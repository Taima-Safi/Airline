﻿using Airline.Database.Model;
using Airline.Dto.Airport;
using Airline.Dto.Book;
using Airline.Dto.Flight;
using Airline.Dto.Seat;
using Airline.Dto.User;
using Airline.Repository;
using Airline.Shared.Enum;
using Airline.Shared.Exception;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Airline.Service.Flight;

public class FlightService : IFlightService
{
    private readonly IBaseRepo<FlightModel> flightBaseRepo;
    private readonly IBaseRepo<AirportModel> airportBaseRepo;
    private readonly IBaseRepo<AirplaneModel> airplaneBaseRepo;
    private readonly IBaseRepo<FlightClassPriceModel> flightClassPriceBaseRepo;


    public FlightService(IBaseRepo<FlightModel> flightBaseRepo, IBaseRepo<AirportModel> airportBaseRepo,
        IBaseRepo<AirplaneModel> airplaneBaseRepo, IBaseRepo<FlightClassPriceModel> flightClassPriceBaseRepo)
    {
        this.flightBaseRepo = flightBaseRepo;
        this.airportBaseRepo = airportBaseRepo;
        this.airplaneBaseRepo = airplaneBaseRepo;
        this.flightClassPriceBaseRepo = flightClassPriceBaseRepo;
    }

    public async Task AddFlightAsync(FlightFormDto dto)
    {

        if (!await airplaneBaseRepo.CheckIfExistAsync(a => a.Id == dto.AirplaneId && a.IsValid))
            throw new NotFoundException("Airplane not found.");

        if (!await airportBaseRepo.CheckIfExistAsync(a => a.Id == dto.ArrivalAirportId && a.IsValid))
            throw new NotFoundException("Arrival Airport not found.");

        if (!await airportBaseRepo.CheckIfExistAsync(a => a.Id == dto.DepartureAirportId && a.IsValid))
            throw new NotFoundException("Departure Airport not found.");

        await flightBaseRepo.AddAsync(new FlightModel
        {
            Code = dto.Code,
            AirplaneId = dto.AirplaneId,
            ArrivalDate = dto.ArrivalDate,
            DepartureDate = dto.DepartureDate,
            ArrivalAirportId = dto.ArrivalAirportId,
            DepartureAirportId = dto.DepartureAirportId,
        });
    }
    public async Task UpdateFlightClassPriceAsync(long flightId, SeatClass type, double price)
    {

        if (!await flightBaseRepo.CheckIfExistAsync(f => f.Id == flightId && f.IsValid))
            throw new NotFoundException("Flight not found.");

        var classPrice = await flightClassPriceBaseRepo.GetByAsync(x => x.FlightId == flightId && x.Type == type && x.IsActive && x.IsValid);

        if (classPrice != null)
            await flightClassPriceBaseRepo.UpdateAsync(f => f.Id == classPrice.Id, setter => setter.SetProperty(x => x.IsActive, false));

        await flightClassPriceBaseRepo.AddAsync(new FlightClassPriceModel
        {
            Type = type,
            Price = price,
            IsActive = true,
            FlightId = flightId
        });

    }
    public async Task<List<FlightDto>> GetAllFlightsAsync(string code, string airplaneTitle, string arrivalCityTitle,
        string departureCityTitle, DateTime? departureDate, DateTime? arrivalDate)
    {
        Expression<Func<FlightModel, bool>> expression = f => (string.IsNullOrEmpty(code) || f.Code.Contains(code))
        && (string.IsNullOrEmpty(departureCityTitle) || f.DepartureAirport.City.Title.Contains(departureCityTitle))
        && (string.IsNullOrEmpty(arrivalCityTitle) || f.ArrivalAirport.City.Title.Contains(arrivalCityTitle))
        && (string.IsNullOrEmpty(airplaneTitle) || f.Airplane.Title.Contains(airplaneTitle))
        && (!departureDate.HasValue || f.DepartureDate == departureDate)
        && (!arrivalDate.HasValue || f.ArrivalDate == arrivalDate)
        && f.IsValid;

        var flights = await flightBaseRepo.GetAllAsync(expression, x => x.Include(x => x.ArrivalAirport).ThenInclude(c => c.City)
        , x => x.Include(x => x.DepartureAirport).ThenInclude(c => c.City), x => x.Include(x => x.Airplane), x => x.Include(x => x.FlightClassPrices));

        return flights.Select(f => new FlightDto
        {
            Id = f.Id,
            Code = f.Code,
            ArrivalDate = f.ArrivalDate,
            DepartureDate = f.DepartureDate,
            FlightClassPrice = f.FlightClassPrices.Where(p => p.IsActive)
            .Select(p => new FlightClassPriceDto
            {
                Type = p.Type,
                Price = p.Price,
                IsActive = p.IsActive,
            }).ToList(),
            Airplane = new AirplaneDto
            {
                Id = f.Airplane.Id,
                Title = f.Airplane.Title,
            },
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
    public async Task<FlightDto> GetFlightByIdAsync(long id, bool justActivePrice)
    {
        Expression<Func<FlightModel, bool>> expression = f => f.Id == id && f.IsValid;
        if (!await flightBaseRepo.CheckIfExistAsync(expression))
            throw new NotFoundException("Flight not found");

        var flight = await flightBaseRepo.GetByAsync(expression, x => x.Include(x => x.ArrivalAirport).ThenInclude(c => c.City)
        , x => x.Include(x => x.DepartureAirport).ThenInclude(c => c.City), x => x.Include(x => x.Airplane).ThenInclude(c => c.Seats).ThenInclude(x => x.Books)
        , x => x.Include(x => x.FlightClassPrices));

        return new FlightDto
        {
            Id = flight.Id,
            Code = flight.Code,
            ArrivalDate = flight.ArrivalDate,
            DepartureDate = flight.DepartureDate,
            Airplane = new AirplaneDto
            {
                Id = flight.Airplane.Id,
                Title = flight.Airplane.Title,
                Seats = flight.Airplane.Seats.Select(s => new SeatDto
                {
                    Id = s.Id,
                    Code = s.Code,
                    Type = s.Type,
                    Books = s.Books.Where(b => b.IsValid).Select(b => new BookDto
                    {
                        Date = b.Date,
                        User = new UserDto
                        {
                            Id = b.User.Id,
                            Email = b.User.Email
                        },
                        BookStatus = b.BookStatus,
                    }).ToList(),
                }).ToList()
            },
            FlightClassPrice = flight.FlightClassPrices.Where(p => justActivePrice ? p.IsActive : !p.IsActive)
            .Select(p => new FlightClassPriceDto
            {
                Type = p.Type,
                Price = p.Price,
                IsActive = p.IsActive,
            }).ToList(),
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
            throw new NotFoundException("Flight not found.");

        if (!await airplaneBaseRepo.CheckIfExistAsync(a => a.Id == dto.AirplaneId && a.IsValid))
            throw new NotFoundException("Airplane not found.");

        if (!await airportBaseRepo.CheckIfExistAsync(a => a.Id == dto.ArrivalAirportId && a.IsValid))
            throw new NotFoundException("Arrival Airport not found.");

        if (!await airportBaseRepo.CheckIfExistAsync(a => a.Id == dto.DepartureAirportId && a.IsValid))
            throw new NotFoundException("Departure Airport not found.");

        model.Code = dto.Code;
        model.AirplaneId = dto.AirplaneId;
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
            throw new NotFoundException("Flight not found.");

        await flightBaseRepo.RemoveAsync(expression,
            setters => setters.SetProperty(x => x.IsValid, false));
    }
}
