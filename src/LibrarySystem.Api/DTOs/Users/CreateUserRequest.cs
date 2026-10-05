using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Api.DTOs.Users;

public class CreateUserRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string Email { get; set; } = string.Empty;
}