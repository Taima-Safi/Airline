using Airline.Service.Country;
using Microsoft.AspNetCore.Mvc;

namespace Airline.Controllers;

[Route("api/[controller]")]
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
}
