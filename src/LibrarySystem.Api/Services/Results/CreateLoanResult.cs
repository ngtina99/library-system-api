using LibrarySystem.Api.DTOs.Loans;

namespace LibrarySystem.Api.Services.Results;

public class CreateLoanResult
{
    public LoanResponse? Loan { get; init; }

    public CreateLoanError? Error { get; init; }
}

public enum CreateLoanError
{
    UserNotFound,
    BookNotFound,
    BookNotAvailable
}