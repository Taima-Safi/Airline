﻿namespace Airline.Database.Model;

public class AirportModel : BaseModel
{
    public string Title { get; set; }
    public long CityId { get; set; }
    public CityModel City { get; set; }
    public ICollection<FlightModel> ArrivalFlights { get; set; }
    public ICollection<FlightModel> DepartureFlights { get; set; }
}
