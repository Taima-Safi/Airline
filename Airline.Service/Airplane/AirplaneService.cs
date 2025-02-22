using Airline.Database.Model;
using Airline.Dto.Seat;
using Airline.Repository;
using Airline.Shared.Enum;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Airline.Service.Seat;

public class AirplaneService : IAirplaneService
{
    private readonly IBaseRepo<SeatModel> seatBaseRepo;
    private readonly IBaseRepo<FlightModel> flightBaseRepo;
    private readonly IBaseRepo<AirplaneModel> airplaneBaseRepo;

    public AirplaneService(IBaseRepo<SeatModel> seatBaseRepo, IBaseRepo<FlightModel> flightBaseRepo, IBaseRepo<AirplaneModel> airplaneBaseRepo)
    {
        this.seatBaseRepo = seatBaseRepo;
        this.flightBaseRepo = flightBaseRepo;
        this.airplaneBaseRepo = airplaneBaseRepo;
    }

    #region Airplane
    public async Task AddAirplaneAsync(string title)
     => await airplaneBaseRepo.AddAsync(new AirplaneModel { Title = title });
    public async Task RemoveAirplaneAsync(long id)
     => await airplaneBaseRepo.RemoveAsync(a => a.Id == id, setter => setter.SetProperty(x => x.IsValid, false));
    #endregion

    #region Seat
    public async Task AddSeatAsync(SeatFormDto dto)
    {

        if (!await airplaneBaseRepo.CheckIfExistAsync(a => a.Id == dto.AirplaneId && a.IsValid))
            throw new ArgumentException("Airplane not found.");

        await seatBaseRepo.AddAsync(new SeatModel
        {
            Code = dto.Code,
            Type = dto.Type,
            AirplaneId = dto.AirplaneId
        });
    }
    public async Task<List<SeatDto>> GetAllAirplaneSeatsAsync(long airplaneId, string seatCode, string airplaneTitle, SeatClass? type)
    {
        Expression<Func<SeatModel, bool>> expression = s => s.AirplaneId == airplaneId
        && (!type.HasValue || s.Type == type)
        && (string.IsNullOrEmpty(seatCode) || s.Code.Contains(seatCode))
        && (string.IsNullOrEmpty(airplaneTitle) || s.Airplane.Title.Contains(airplaneTitle))
        && s.IsValid;

        var seats = await seatBaseRepo.GetAllAsync(expression, x => x.Include(x => x.Airplane));

        return seats.Select(s => new SeatDto
        {
            Id = s.Id,
            Code = s.Code,
            Type = s.Type,
            AirplaneId = s.Airplane.Id,
            AirplaneTitle = s.Airplane.Title
        }).ToList();
    }
    public async Task<SeatDto> GetSeatAsync(long seatId)
    {
        Expression<Func<SeatModel, bool>> expression = s => s.Id == seatId && s.IsValid;

        if (!await seatBaseRepo.CheckIfExistAsync(expression))
            throw new ArgumentException("Airplane seat not found.");

        var seat = await seatBaseRepo.GetByAsync(expression, x => x.Include(x => x.Airplane));

        return new SeatDto
        {
            Id = seat.Id,
            Code = seat.Code,
            Type = seat.Type,
            AirplaneId = seat.Airplane.Id,
            AirplaneTitle = seat.Airplane.Title
        };
    }
    public async Task UpdateSeatAsync(long seatId, SeatFormDto dto)
    {
        var model = await seatBaseRepo.GetByAsync(x => x.Id == seatId && x.IsValid) ??
            throw new ArgumentException("seat not found.");

        if (!await airplaneBaseRepo.CheckIfExistAsync(a => a.Id == dto.AirplaneId && a.IsValid))
            throw new ArgumentException(" Airplane not found.");

        await seatBaseRepo.UpdateAsync(
        f => f.Id == seatId && f.IsValid,
        setters => setters
            .SetProperty(x => x.Code, dto.Code)
            .SetProperty(x => x.AirplaneId, dto.AirplaneId)
            .SetProperty(x => x.Type, dto.Type));
    }
    public async Task RemoveSeatAsync(long seatId)
    {
        Expression<Func<SeatModel, bool>> expression = s => s.Id == seatId && s.IsValid;

        if (!await seatBaseRepo.CheckIfExistAsync(expression))
            throw new ArgumentException("Airplane seat not found.");

        await seatBaseRepo.RemoveAsync(expression,
            setters => setters.SetProperty(x => x.IsValid, false));
    }
    #endregion
}
