using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Api.DTOs.Loans;

public class CreateLoanRequest
{
    [Required]
    public Guid? UserId { get; set; }

    [Required]
    public Guid? BookId { get; set; }
}