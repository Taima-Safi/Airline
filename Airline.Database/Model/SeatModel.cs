using Airline.Shared.Enum;

namespace Airline.Database.Model;

public class SeatModel : BaseModel
{
    public string Code { get; set; }
    public SeatClass Type { get; set; }

    public long AirplaneId { get; set; }
    public AirplaneModel Airplane { get; set; }
    public ICollection<BookModel> Books { get; set; }
}
