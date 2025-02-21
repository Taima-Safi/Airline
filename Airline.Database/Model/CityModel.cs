namespace Airline.Database.Model;

public class CityModel : BaseModel
{
    public string Title { get; set; }
    public long CountryId { get; set; }
    public CountryModel Country { get; set; }
}
