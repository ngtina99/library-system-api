using LibrarySystem.Api.DTOs.Books;
using LibrarySystem.Api.Services.Interfaces;
using LibrarySystem.Api.Services.Results;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpPost]
    public async Task<ActionResult<BookResponse>> Create(
        CreateBookRequest request)
    {
        var book = await _bookService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = book.Id },
            book);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookResponse>> GetById(Guid id)
    {
        var book = await _bookService.GetByIdAsync(id);

        if (book is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Book not found",
                detail: "The specified book does not exist.");
        }

        return Ok(book);
    }

    [HttpGet]
    public async Task<ActionResult<List<BookResponse>>> GetAll(
        [FromQuery] bool? available,
        [FromQuery] string? author)
    {
        var books = await _bookService.GetAllAsync(
            available,
            author);

        return Ok(books);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BookResponse>> Update(
        Guid id,
        UpdateBookRequest request)
    {
        var book = await _bookService.UpdateAsync(id, request);

        if (book is null)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Book not found",
                detail: "The specified book does not exist.");
        }

        return Ok(book);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _bookService.DeleteAsync(id);

        return result switch
        {
            DeleteBookResult.Success =>
                NoContent(),

            DeleteBookResult.NotFound =>
                Problem(
                    statusCode: StatusCodes.Status404NotFound,
                    title: "Book not found",
                    detail: "The specified book does not exist."),

            DeleteBookResult.BookOnLoan =>
                Problem(
                    statusCode: StatusCodes.Status409Conflict,
                    title: "Book is currently on loan",
                    detail: "The book cannot be deleted while it is on loan."),

            _ =>
                StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}