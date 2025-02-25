using Airline.Database.Model;
using Airline.Dto.Book;
using Airline.Dto.Flight;
using Airline.Dto.Seat;
using Airline.Dto.User;
using Airline.Repository;
using Airline.Repository.Flight;
using Airline.Shared.Enum;
using Airline.Shared.Exception;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Linq.Expressions;
using System.Security.Cryptography;

namespace Airline.Service.Book;

public class BookService : IBookService
{
    private readonly IFlightRepo flightRepo;
    private readonly IConfiguration _configuration;
    private readonly IBaseRepo<BookModel> bookBaseRepo;
    private readonly IBaseRepo<UserModel> userBaseRepo;
    private readonly IBaseRepo<SeatModel> seatBaseRepo;
    private readonly IBaseRepo<FlightModel> flightBaseRepo;
    private readonly IBaseRepo<FlightClassPriceModel> flightClassPriceBaseRepo;
    public BookService(IBaseRepo<FlightModel> flightBaseRepo, IBaseRepo<FlightClassPriceModel> flightClassPriceBaseRepo,
        IBaseRepo<BookModel> bookBaseRepo, IBaseRepo<UserModel> userBaseRepo, IBaseRepo<SeatModel> seatBaseRepo, IFlightRepo flightRepo, IConfiguration configuration)
    {
        this.flightBaseRepo = flightBaseRepo;
        this.flightClassPriceBaseRepo = flightClassPriceBaseRepo;
        this.bookBaseRepo = bookBaseRepo;
        this.userBaseRepo = userBaseRepo;
        this.seatBaseRepo = seatBaseRepo;
        this.flightRepo = flightRepo;
        _configuration = configuration;
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
        //Start transaction
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
        var email = await flightRepo.GetUserEmailAsync(userBookCode);
        //await SendBookingConfirmation(email, "Pending");
        _ = Task.Run(() => SendBookingConfirmation(email, "Pending"));
        //End transaction
        return userBookCode;
    }
    public async Task<List<BookDto>> GetAllFlightBooksAsync(long flightId, string userBookCode, bool? isConfirmed)
    {
        Expression<Func<BookModel, bool>> expression = b => b.FlightId == flightId
                && (string.IsNullOrEmpty(userBookCode) || b.UserBookCode.Contains(userBookCode))
                && (!isConfirmed.HasValue || b.BookStatus == BookStatus.Confirmed)
                && b.IsValid;

        var books = await bookBaseRepo.GetAllAsync(expression, x => x.Include(x => x.User)
        , x => x.Include(x => x.Seat), x => x.Include(x => x.Flight));

        return books.Select(f => new BookDto
        {
            Id = f.Id,
            Date = f.Date,
            Price = f.Price,
            BookStatus = f.BookStatus,
            Seat = new SeatDto
            {
                Id = f.Seat.Id,
                Code = f.Seat.Code,
            },
            Flight = new FlightDto
            {
                ArrivalDate = f.Flight.ArrivalDate,
                DepartureDate = f.Flight.DepartureDate,
            },
            User = new UserDto
            {
                Id = f.User.Id,
                Email = f.User.Email,
            }
        }).ToList();
    }
    public async Task UpdateBookStatusAsync(string userBookCode, bool isConfirmed)
    {
        if (!await bookBaseRepo.CheckIfExistAsync(b => b.UserBookCode == userBookCode && b.IsValid))
            throw new NotFoundException("user book not found");
        var email = await flightRepo.GetUserEmailAsync(userBookCode);

        //Check if payment has been made successfully then => 
        if (isConfirmed)
        {
            await bookBaseRepo.UpdateAsync(b => b.UserBookCode == userBookCode && b.IsValid, setter => setter.SetProperty(x => x.BookStatus, BookStatus.Confirmed));
            //await SendBookingConfirmation(email, "Confirmed");
            _ = Task.Run(() => SendBookingConfirmation(email, "Pending"));

        }
        else
        {
            // if should return money should update payment model
            await bookBaseRepo.UpdateAsync(b => b.UserBookCode == userBookCode && b.IsValid, setter => setter.SetProperty(x => x.BookStatus, BookStatus.Canceled));
            _ = Task.Run(() => SendBookingConfirmation(email, "Canceled"));
        }
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
    public async Task SendBookingConfirmation(string toEmail, string status)
    {
        var apiKey = _configuration["SendGrid:ApiKey"];
        var client = new SendGridClient(apiKey);

        var from = new EmailAddress(_configuration["SendGrid:FromEmail"], _configuration["SendGrid:FromName"]);
        var to = new EmailAddress(toEmail);
        var subject = "Booking Confirmed";

        var msg = MailHelper.CreateSingleEmail(from, to, subject, $"Your book is {status} successfully", $"{toEmail}");
        var response = await client.SendEmailAsync(msg);

        if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
        {
            throw new ArgumentException("An error occur when sending email, please try later..");
        }
    }
}
