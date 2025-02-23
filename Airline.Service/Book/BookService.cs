using Airline.Database.Model;
using Airline.Dto.Book;
using Airline.Repository;
using Airline.Repository.Flight;
using Airline.Shared.Exception;

namespace Airline.Service.Book;

public class BookService : IBookService
{
    private readonly IFlightRepo flightRepo;
    private readonly IBaseRepo<BookModel> bookBaseRepo;
    private readonly IBaseRepo<UserModel> userBaseRepo;
    private readonly IBaseRepo<SeatModel> seatBaseRepo;
    private readonly IBaseRepo<FlightModel> flightBaseRepo;
    private readonly IBaseRepo<FlightClassPriceModel> flightClassPriceBaseRepo;
    public BookService(IBaseRepo<FlightModel> flightBaseRepo, IBaseRepo<FlightClassPriceModel> flightClassPriceBaseRepo,
        IBaseRepo<BookModel> bookBaseRepo, IBaseRepo<UserModel> userBaseRepo, IBaseRepo<SeatModel> seatBaseRepo, IFlightRepo flightRepo)
    {
        this.flightBaseRepo = flightBaseRepo;
        this.flightClassPriceBaseRepo = flightClassPriceBaseRepo;
        this.bookBaseRepo = bookBaseRepo;
        this.userBaseRepo = userBaseRepo;
        this.seatBaseRepo = seatBaseRepo;
        this.flightRepo = flightRepo;
    }

    public async Task AddBookAsync(BookFormDto dto)
    {
        var isFounded = await userBaseRepo.CheckIfExistAsync(a => a.Id == dto.UserId && a.IsValid)
            && await seatBaseRepo.CheckIfExistAsync(a => a.Id == dto.SeatId && a.IsValid)
            && await flightBaseRepo.CheckIfExistAsync(a => a.Id == dto.FlightId && a.IsValid);

        if (!isFounded)
            throw new NotFoundException("User, Flight or Seat is not found.");

        if (await bookBaseRepo.CheckIfExistAsync(a => a.FlightId == dto.FlightId && a.SeatId == dto.SeatId && a.IsValid))
            throw new ArgumentException("Sorry.. Seat Booked.");

        var seatType = await flightRepo.GetSeatTypeAsync(dto.SeatId);
        var price = await flightRepo.GetSeatPriceAsync(seatType, dto.FlightId);

        await bookBaseRepo.AddAsync(new BookModel
        {
            Price = price,
            Date = dto.Date,
            SeatId = dto.SeatId,
            UserId = dto.UserId,
            FlightId = dto.FlightId,
        });
    }

}
