namespace Airline.Dto.Airport
{
    public class AirportDto
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public long CityId { get; set; }
        public string CityTitle { get; set; }
        // public ICollection<FlightModel> ArrivalFlights { get; set; }
        // public ICollection<FlightModel> DepartureFlights { get; set; }
    }
}
