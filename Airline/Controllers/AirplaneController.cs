using Airline.Dto.Seat;
using Airline.Service.Seat;
using Airline.Shared.Enum;
using Microsoft.AspNetCore.Mvc;

namespace Airline.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AirplaneController : ControllerBase
    {
        private readonly IAirplaneService airplaneService;

        public AirplaneController(IAirplaneService airplaneService)
        {
            this.airplaneService = airplaneService;
        }
        #region Airplane
        [HttpPost]
        public async Task<IActionResult> AddAirplane(string title)
        {
            await airplaneService.AddAirplaneAsync(title);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> RemoveAirplane(long id)
        {
            await airplaneService.RemoveAirplaneAsync(id);
            return Ok();
        }
        #endregion

        #region Seat
        [HttpPost]
        public async Task<IActionResult> AddSeat(SeatFormDto dto)
        {
            await airplaneService.AddSeatAsync(dto);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> AddSeats(List<string> seatCode, SeatClass type, long airplaneId)
        {
            await airplaneService.AddSeatsAsync(seatCode, type, airplaneId);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAirplaneSeats(long airplaneId, string seatCode, string airplaneTitle, SeatClass? type)
        {
            var seats = await airplaneService.GetAllAirplaneSeatsAsync(airplaneId, seatCode, airplaneTitle, type);
            return Ok(seats);
        }
        [HttpGet]
        public async Task<IActionResult> GetSeat(long seatId)
        {
            var seat = await airplaneService.GetSeatAsync(seatId);
            return Ok(seat);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateSeat(long seatId, SeatFormDto dto)
        {
            await airplaneService.UpdateSeatAsync(seatId, dto);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> RemoveSeat(long seatId)
        {
            await airplaneService.RemoveSeatAsync(seatId);
            return Ok();
        }
        #endregion
    }
}
