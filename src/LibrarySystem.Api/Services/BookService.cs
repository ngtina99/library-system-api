using LibrarySystem.Api.Data;
using LibrarySystem.Api.DTOs.Books;
using LibrarySystem.Api.Models;
using LibrarySystem.Api.Services.Interfaces;
using LibrarySystem.Api.Services.Results;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Api.Services;

public class BookService : IBookService
{
    private readonly LibraryDbContext _context;

    public BookService(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<BookResponse> CreateAsync(CreateBookRequest request)
    {
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Author = request.Author,
            ISBN = request.ISBN,
            PublishedYear = request.PublishedYear,
            IsAvailable = true
        };

        _context.Books.Add(book);

        await _context.SaveChangesAsync();

        return new BookResponse
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            ISBN = book.ISBN,
            PublishedYear = book.PublishedYear,
            IsAvailable = book.IsAvailable
        };
    }

    public async Task<BookResponse?> GetByIdAsync(Guid id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book is null)
        {
            return null;
        }

        return new BookResponse
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            ISBN = book.ISBN,
            PublishedYear = book.PublishedYear,
            IsAvailable = book.IsAvailable
        };
    }

    public async Task<List<BookResponse>> GetAllAsync(
        bool? available,
        string? author)
    {
        var query = _context.Books
            .AsNoTracking();

        if (available.HasValue)
        {
            query = query.Where(book =>
                book.IsAvailable == available.Value);
        }

        if (!string.IsNullOrWhiteSpace(author))
        {
            query = query.Where(book =>
                book.Author.Contains(author));
        }

        return await query
            .Select(book => new BookResponse
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                PublishedYear = book.PublishedYear,
                IsAvailable = book.IsAvailable
            })
            .ToListAsync();
    }

    public async Task<BookResponse?> UpdateAsync(
        Guid id,
        UpdateBookRequest request)
    {
        var book = await _context.Books.FindAsync(id);

        if (book is null)
        {
            return null;
        }

        book.Title = request.Title;
        book.Author = request.Author;
        book.ISBN = request.ISBN;
        book.PublishedYear = request.PublishedYear;

        await _context.SaveChangesAsync();

        return new BookResponse
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            ISBN = book.ISBN,
            PublishedYear = book.PublishedYear,
            IsAvailable = book.IsAvailable
        };
    }

    public async Task<DeleteBookResult> DeleteAsync(Guid id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book is null)
        {
            return DeleteBookResult.NotFound;
        }

        var hasActiveLoan = await _context.Loans.AnyAsync(
            loan => loan.BookId == id && loan.ReturnDate == null);

        if (hasActiveLoan)
        {
            return DeleteBookResult.BookOnLoan;
        }

        _context.Books.Remove(book);

        await _context.SaveChangesAsync();

        return DeleteBookResult.Success;
    }
}