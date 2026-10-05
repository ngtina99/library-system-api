using LibrarySystem.Api.Data;
using LibrarySystem.Api.Models;
using LibrarySystem.Api.Services;
using LibrarySystem.Api.Services.Results;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Api.Tests.Services;

public class BookServiceTests
{
    [Fact]
    public async Task DeleteAsync_WhenBookHasActiveLoan_ShouldReturnBookOnLoan()
    {
        // Arrange
        await using var context = CreateDbContext();

        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = "Clean Code",
            Author = "Robert C. Martin",
            ISBN = "9780132350884",
            PublishedYear = 2008,
            IsAvailable = false
        };

        var loan = new Loan
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BookId = book.Id,
            LoanDate = DateTimeOffset.UtcNow,
            ReturnDate = null
        };

        context.Books.Add(book);
        context.Loans.Add(loan);

        await context.SaveChangesAsync();

        var service = new BookService(context);

        // Act
        var result = await service.DeleteAsync(book.Id);

        // Assert
        Assert.Equal(DeleteBookResult.BookOnLoan, result);

        Assert.NotNull(
            await context.Books.FindAsync(book.Id));
    }

    [Fact]
    public async Task DeleteAsync_WhenBookHasNoActiveLoan_ShouldDeleteBook()
    {
        // Arrange
        await using var context = CreateDbContext();

        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = "Clean Code",
            Author = "Robert C. Martin",
            ISBN = "9780132350884",
            PublishedYear = 2008,
            IsAvailable = true
        };

        context.Books.Add(book);

        await context.SaveChangesAsync();

        var service = new BookService(context);

        // Act
        var result = await service.DeleteAsync(book.Id);

        // Assert
        Assert.Equal(DeleteBookResult.Success, result);

        Assert.Null(
            await context.Books.FindAsync(book.Id));
    }

    private static LibraryDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new LibraryDbContext(options);
    }
}