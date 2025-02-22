
using Airline.Dto.Seat;
using Airline.Shared.Enum;

namespace Airline.Service.Seat;

public interface IAirplaneService
{
    Task AddAirplaneAsync(string title);
    Task AddSeatAsync(SeatFormDto dto);
    Task<List<SeatDto>> GetAllAirplaneSeatsAsync(long airplaneId, string seatCode, string airplaneTitle, SeatClass? type);
    Task<SeatDto> GetSeatAsync(long seatId);
    Task RemoveAirplaneAsync(long id);
    Task RemoveSeatAsync(long seatId);
    Task UpdateSeatAsync(long seatId, SeatFormDto dto);
}
