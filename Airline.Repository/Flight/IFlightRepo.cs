using Airline.Shared.Enum;

namespace Airline.Repository.Flight;

public interface IFlightRepo
{
    Task<double> GetSeatPriceAsync(SeatClass type, long flightId);
    Task<SeatClass> GetSeatTypeAsync(long seatId);
    Task<string> GetUserEmailAsync(string userBookingCode);
}
