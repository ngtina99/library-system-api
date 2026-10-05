namespace LibrarySystem.Api.Models;

public class User
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public required string Email { get; set; }

    public DateTimeOffset RegisteredDate { get; set; } = DateTimeOffset.UtcNow;
}