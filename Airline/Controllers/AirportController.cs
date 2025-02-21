using Airline.Service.Airport;
using Microsoft.AspNetCore.Mvc;

namespace Airline.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AirportController : ControllerBase
    {
        private readonly IAirportService airportService;

        public AirportController(IAirportService airportService)
        {
            this.airportService = airportService;
        }

        public async Task<IActionResult> AddAirportAsync(string title, long cityId)
        {
            await airportService.AddAirportAsync(title, cityId);
            return Ok();
        }
    }
}
