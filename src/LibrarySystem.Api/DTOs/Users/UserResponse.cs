namespace LibrarySystem.Api.DTOs.Users;

public class UserResponse
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Email { get; set; }

    public DateTimeOffset RegisteredDate { get; set; }
}