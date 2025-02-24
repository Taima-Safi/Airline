using Airline.Database.Model;
using Airline.Dto.Book;
using Airline.Repository;
using Airline.Repository.Flight;
using Airline.Shared.Enum;
using Airline.Shared.Exception;
using System.Security.Cryptography;

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

    public async Task<string> AddBookAsync(BookFormDto dto)
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

        var userBookCode = GenerateRandomString();
        await bookBaseRepo.AddAsync(new BookModel
        {
            Price = price,
            Date = dto.Date,
            SeatId = dto.SeatId,
            UserId = dto.UserId,
            FlightId = dto.FlightId,
            UserBookCode = userBookCode,
            BookStatus = BookStatus.Pending // when payment complete then BookStatus = Confirmed
        });
        return userBookCode;
    }
    public async Task UpdateBookStatusAsync(string userBookCode, bool isConfirmed)
    {
        if (!await bookBaseRepo.CheckIfExistAsync(b => b.UserBookCode == userBookCode && b.IsValid))
            throw new NotFoundException("user book not found");

        //Check if payment has been made successfully then => 
        if (isConfirmed)
            await bookBaseRepo.UpdateAsync(b => b.UserBookCode == userBookCode && b.IsValid, setter => setter.SetProperty(x => x.BookStatus, BookStatus.Confirmed));
        else
            // if should return money should update payment model
            await bookBaseRepo.UpdateAsync(b => b.UserBookCode == userBookCode && b.IsValid, setter => setter.SetProperty(x => x.BookStatus, BookStatus.Canceled));
    }
    public async Task RemoveBookAsync(long bookId)
    {

        if (!await bookBaseRepo.CheckIfExistAsync(b => b.Id == bookId && b.IsValid))
            throw new NotFoundException("user book not found");

        await bookBaseRepo.UpdateAsync(b => b.Id == bookId && b.IsValid, setter => setter.SetProperty(x => x.IsValid, false));
    }

    private static string GenerateRandomString()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        using var rng = RandomNumberGenerator.Create();
        var randomNumber = new byte[5];
        rng.GetBytes(randomNumber);

        // Convert the random bytes to a string using the character pool
        return new string(randomNumber.Select(b => chars[b % chars.Length]).ToArray());

    }
}
