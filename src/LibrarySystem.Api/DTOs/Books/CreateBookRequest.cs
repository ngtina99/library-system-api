using System.ComponentModel.DataAnnotations;
using LibrarySystem.Api.Validation;

namespace LibrarySystem.Api.DTOs.Books;

public class CreateBookRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(150)]
    public string Author { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string ISBN { get; set; } = string.Empty;

    [Range(1, 9999)]
    [NotFutureYear(ErrorMessage = "Published year cannot be in the future.")]
    public int PublishedYear { get; set; }
}