using LibrarySystem.Api.DTOs.Loans;

namespace LibrarySystem.Api.Services.Results;

public class ReturnLoanResult
{
    public LoanResponse? Loan { get; init; }

    public ReturnLoanError? Error { get; init; }
}

public enum ReturnLoanError
{
    LoanNotFound,
    LoanAlreadyReturned,
    BookNotFound
}