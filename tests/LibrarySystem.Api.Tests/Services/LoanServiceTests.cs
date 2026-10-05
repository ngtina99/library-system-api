using LibrarySystem.Api.Data;
using LibrarySystem.Api.DTOs.Loans;
using LibrarySystem.Api.Models;
using LibrarySystem.Api.Services;
using Microsoft.EntityFrameworkCore;
using LibrarySystem.Api.Services.Results;

namespace LibrarySystem.Api.Tests.Services;

public class LoanServiceTests
{
    [Fact]
    public async Task CreateAsync_WhenBookIsAvailable_ShouldCreateLoan()
    {
        // Arrange
        await using var context = CreateDbContext();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com"
        };

        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = "Clean Code",
            Author = "Robert C. Martin",
            ISBN = "9780132350884",
            PublishedYear = 2008,
            IsAvailable = true
        };

        context.Users.Add(user);
        context.Books.Add(book);

        await context.SaveChangesAsync();

        var service = new LoanService(context);

        var request = new CreateLoanRequest
        {
            UserId = user.Id,
            BookId = book.Id
        };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.NotNull(result.Loan);
        Assert.Null(result.Error);

        Assert.Equal(user.Id, result.Loan.UserId);
        Assert.Equal(book.Id, result.Loan.BookId);

        Assert.False(book.IsAvailable);

        Assert.Single(context.Loans);
    }

    [Fact]
    public async Task CreateAsync_WhenBookIsUnavailable_ShouldReturnBookNotAvailable()
    {
        // Arrange
        await using var context = CreateDbContext();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = "Test User",
            Email = "test@example.com"
        };

        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = "Clean Code",
            Author = "Robert C. Martin",
            ISBN = "9780132350884",
            PublishedYear = 2008,
            IsAvailable = false
        };

        context.Users.Add(user);
        context.Books.Add(book);

        await context.SaveChangesAsync();

        var service = new LoanService(context);

        var request = new CreateLoanRequest
        {
            UserId = user.Id,
            BookId = book.Id
        };

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        Assert.Null(result.Loan);

        Assert.Equal(
            CreateLoanError.BookNotAvailable,
            result.Error);

        Assert.Empty(context.Loans);
    }

    [Fact]
    public async Task ReturnAsync_WhenLoanIsActive_ShouldReturnLoan()
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
            LoanDate = DateTimeOffset.UtcNow.AddDays(-1),
            ReturnDate = null
        };

        context.Books.Add(book);
        context.Loans.Add(loan);

        await context.SaveChangesAsync();

        var service = new LoanService(context);

        // Act
        var result = await service.ReturnAsync(loan.Id);

        // Assert
        Assert.NotNull(result.Loan);
        Assert.Null(result.Error);

        Assert.NotNull(loan.ReturnDate);
        Assert.True(book.IsAvailable);
    }

    [Fact]
    public async Task ReturnAsync_WhenLoanAlreadyReturned_ShouldReturnError()
    {
        // Arrange
        await using var context = CreateDbContext();

        var loan = new Loan
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            BookId = Guid.NewGuid(),
            LoanDate = DateTimeOffset.UtcNow.AddDays(-2),
            ReturnDate = DateTimeOffset.UtcNow.AddDays(-1)
        };

        context.Loans.Add(loan);

        await context.SaveChangesAsync();

        var service = new LoanService(context);

        // Act
        var result = await service.ReturnAsync(loan.Id);

        // Assert
        Assert.Null(result.Loan);

        Assert.Equal(
            ReturnLoanError.LoanAlreadyReturned,
            result.Error);
    }

    private static LibraryDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new LibraryDbContext(options);
    }
}