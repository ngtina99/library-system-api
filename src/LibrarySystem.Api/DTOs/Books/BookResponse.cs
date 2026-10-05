namespace LibrarySystem.Api.DTOs.Books;

public class BookResponse
{
    public Guid Id { get; set; }

    public required string Title { get; set; }

    public required string Author { get; set; }

    public required string ISBN { get; set; }

    public int PublishedYear { get; set; }

    public bool IsAvailable { get; set; }
}