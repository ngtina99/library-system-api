using LibrarySystem.Api.Data;
using LibrarySystem.Api.DTOs.Loans;
using LibrarySystem.Api.Models;
using LibrarySystem.Api.Services.Interfaces;
using LibrarySystem.Api.Services.Results;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Api.Services;

public class LoanService : ILoanService
{
    private readonly LibraryDbContext _context;

    public LoanService(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<CreateLoanResult> CreateAsync(
        CreateLoanRequest request)
    {
        var userId = request.UserId!.Value;
        var bookId = request.BookId!.Value;

        var userExists = await _context.Users
            .AnyAsync(user => user.Id == userId);

        if (!userExists)
        {
            return new CreateLoanResult
            {
                Error = CreateLoanError.UserNotFound
            };
        }

        var book = await _context.Books.FindAsync(bookId);

        if (book is null)
        {
            return new CreateLoanResult
            {
                Error = CreateLoanError.BookNotFound
            };
        }

        if (!book.IsAvailable)
        {
            return new CreateLoanResult
            {
                Error = CreateLoanError.BookNotAvailable
            };
        }

        var loan = new Loan
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            BookId = bookId,
            LoanDate = DateTimeOffset.UtcNow,
            ReturnDate = null
        };

        book.IsAvailable = false;

        _context.Loans.Add(loan);

        await _context.SaveChangesAsync();

        return new CreateLoanResult
        {
            Loan = new LoanResponse
            {
                Id = loan.Id,
                UserId = loan.UserId,
                BookId = loan.BookId,
                LoanDate = loan.LoanDate,
                ReturnDate = loan.ReturnDate
            }
        };
    }

    public async Task<LoanResponse?> GetByIdAsync(Guid id)
    {
        return await _context.Loans
            .AsNoTracking()
            .Where(loan => loan.Id == id)
            .Select(loan => new LoanResponse
            {
                Id = loan.Id,
                UserId = loan.UserId,
                BookId = loan.BookId,
                LoanDate = loan.LoanDate,
                ReturnDate = loan.ReturnDate
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<LoanResponse>> GetActiveAsync()
    {
        return await _context.Loans
            .AsNoTracking()
            .Where(loan => loan.ReturnDate == null)
            .Select(loan => new LoanResponse
            {
                Id = loan.Id,
                UserId = loan.UserId,
                BookId = loan.BookId,
                LoanDate = loan.LoanDate,
                ReturnDate = loan.ReturnDate
            })
            .ToListAsync();
    }

    public async Task<ReturnLoanResult> ReturnAsync(Guid loanId)
    {
        var loan = await _context.Loans.FindAsync(loanId);

        if (loan is null)
        {
            return new ReturnLoanResult
            {
                Error = ReturnLoanError.LoanNotFound
            };
        }

        if (loan.ReturnDate is not null)
        {
            return new ReturnLoanResult
            {
                Error = ReturnLoanError.LoanAlreadyReturned
            };
        }

        var book = await _context.Books.FindAsync(loan.BookId);

        if (book is null)
        {
            return new ReturnLoanResult
            {
                Error = ReturnLoanError.BookNotFound
            };
        }

        loan.ReturnDate = DateTimeOffset.UtcNow;
        book.IsAvailable = true;

        await _context.SaveChangesAsync();

        return new ReturnLoanResult
        {
            Loan = new LoanResponse
            {
                Id = loan.Id,
                UserId = loan.UserId,
                BookId = loan.BookId,
                LoanDate = loan.LoanDate,
                ReturnDate = loan.ReturnDate
            }
        };
    }
}