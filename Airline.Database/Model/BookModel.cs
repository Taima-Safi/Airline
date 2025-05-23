﻿using Airline.Shared.Enum;

namespace Airline.Database.Model;

public class BookModel : BaseModel
{
    public double Price { get; set; }
    public DateTime Date { get; set; }
    public string UserBookCode { get; set; }
    public BookStatus BookStatus { get; set; }

    public long SeatId { get; set; }
    public SeatModel Seat { get; set; }

    public long UserId { get; set; }
    public UserModel User { get; set; }

    public long FlightId { get; set; }
    public FlightModel Flight { get; set; }
}
