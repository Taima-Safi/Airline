using Airline.Database.Context;
using Airline.Shared.Enum;
using Microsoft.EntityFrameworkCore;

namespace Airline.Repository.Flight;

public class FlightRepo : IFlightRepo
{
    private readonly AirlineDbContext context;

    public FlightRepo(AirlineDbContext context)
    {
        this.context = context;
    }
    public async Task<double> GetSeatPriceAsync(SeatClass type, long flightId)
        => await context.FlightClassPrice.Where(s => s.FlightId == flightId && s.Type == type && s.IsActive && s.IsValid).Select(s => s.Price).FirstOrDefaultAsync();
    public async Task<SeatClass> GetSeatTypeAsync(long seatId)
        => await context.Seat.Where(s => s.Id == seatId && s.IsValid).Select(s => s.Type).FirstOrDefaultAsync();
}
