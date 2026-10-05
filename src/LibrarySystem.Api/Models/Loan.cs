namespace LibrarySystem.Api.Models;

public class Loan
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid BookId { get; set; }

    public DateTimeOffset LoanDate { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? ReturnDate { get; set; }
}