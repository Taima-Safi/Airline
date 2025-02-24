using Airline.Dto.Book;
using Airline.Service.Book;
using Microsoft.AspNetCore.Mvc;

namespace Airline.Controllers;

[Route("api/[controller]/[action]")]
[ApiController]
public class BookController : ControllerBase
{
    private readonly IBookService bookService;

    public BookController(IBookService bookService)
    {
        this.bookService = bookService;
    }
    [HttpPost]
    public async Task<IActionResult> AddBook(BookFormDto dto)
    {
        var userBookCode = await bookService.AddBookAsync(dto);
        return Ok(userBookCode);
    }
    [HttpPost]
    public async Task<IActionResult> UpdateBookStatus(string userBookCode, bool isConfirmed)
    {
        await bookService.UpdateBookStatusAsync(userBookCode, isConfirmed);
        return Ok();
    }
    [HttpPost]
    public async Task<IActionResult> RemoveBook(long bookId)
    {
        await bookService.RemoveBookAsync(bookId);
        return Ok();
    }
}
