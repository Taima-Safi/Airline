﻿using Airline.Dto.Flight;
using Airline.Service.Flight;
using Airline.Shared.Enum;
using Microsoft.AspNetCore.Mvc;

namespace Airline.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class FlightController : ControllerBase
{
    private readonly IFlightService flightService;

    public FlightController(IFlightService flightService)
    {
        this.flightService = flightService;
    }
    [HttpPost]
    public async Task<IActionResult> AddFlight(FlightFormDto dto)
    {
        await flightService.AddFlightAsync(dto);
        return Ok();
    }
    [HttpPost]
    public async Task<IActionResult> UpdateFlightClassPrice(long flightId, SeatClass type, double price)
    {
        await flightService.UpdateFlightClassPriceAsync(flightId, type, price);
        return Ok();
    }
    [HttpGet]
    public async Task<IActionResult> GetFlights(string code, string airplaneTitle, string arrivalCityTitle, string departureCityTitle, DateTime? departureDate, DateTime? arrivalDate)
    {
        var flights = await flightService.GetAllFlightsAsync(code, airplaneTitle, arrivalCityTitle, departureCityTitle, departureDate, arrivalDate);
        return Ok(flights);
    }
    [HttpGet]
    public async Task<IActionResult> GetFlight(long id, bool justActivePrice)
    {
        var flight = await flightService.GetFlightByIdAsync(id, justActivePrice);
        return Ok(flight);
    }
    [HttpPost]
    public async Task<IActionResult> UpdateFlight(long flightId, FlightFormDto dto)
    {
        await flightService.UpdateFlightAsync(flightId, dto);
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> RemoveFlight(long flightId)
    {
        await flightService.RemoveFlightAsync(flightId);
        return Ok();
    }
}
