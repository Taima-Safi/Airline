using Airline.Service.Country;
using Microsoft.AspNetCore.Mvc;

namespace Airline.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class CountryController : ControllerBase
{
    private readonly ICountryService countryService;

    public CountryController(ICountryService countryService)
    {
        this.countryService = countryService;
    }
    [HttpPost]
    public async Task<IActionResult> AddCountry(string title)
    {
        await countryService.AddCountryAsync(title);
        return Ok();
    }
    [HttpPost]
    public async Task<IActionResult> AddCity(string title, long countryId)
    {
        await countryService.AddCityAsync(title, countryId);
        return Ok();
    }
}
