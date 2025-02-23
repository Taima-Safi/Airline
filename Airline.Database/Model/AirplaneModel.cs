namespace Airline.Database.Model;
public class AirplaneModel : BaseModel
{
    public string Title { get; set; }
    public ICollection<SeatModel> Seats { get; set; } =
    [];
}
