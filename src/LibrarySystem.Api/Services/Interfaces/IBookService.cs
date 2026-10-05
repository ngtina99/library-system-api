using LibrarySystem.Api.DTOs.Books;
using LibrarySystem.Api.Services.Results;

namespace LibrarySystem.Api.Services.Interfaces;

public interface IBookService
{
    Task<BookResponse> CreateAsync(CreateBookRequest request);
    Task<BookResponse?> GetByIdAsync(Guid id);

    Task<List<BookResponse>> GetAllAsync(
        bool? available,
        string? author);

    Task<BookResponse?> UpdateAsync(
        Guid id,
        UpdateBookRequest request);

    Task<DeleteBookResult> DeleteAsync(Guid id);
}