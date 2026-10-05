using LibrarySystem.Api.DTOs.Loans;
using LibrarySystem.Api.Services.Results;

namespace LibrarySystem.Api.Services.Interfaces;

public interface ILoanService
{
    Task<CreateLoanResult> CreateAsync(CreateLoanRequest request);

    Task<ReturnLoanResult> ReturnAsync(Guid loanId);

    Task<List<LoanResponse>> GetActiveAsync();

    Task<LoanResponse?> GetByIdAsync(Guid id);
}