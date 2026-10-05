namespace LibrarySystem.Api.DTOs.Loans;

public class LoanResponse
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid BookId { get; set; }

    public DateTimeOffset LoanDate { get; set; }

    public DateTimeOffset? ReturnDate { get; set; }
}