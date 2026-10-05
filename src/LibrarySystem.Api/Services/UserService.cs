using LibrarySystem.Api.Data;
using LibrarySystem.Api.DTOs.Users;
using LibrarySystem.Api.Models;
using LibrarySystem.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Api.Services;

public class UserService : IUserService
{
    private readonly LibraryDbContext _context;

    public UserService(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<UserResponse> CreateAsync(
        CreateUserRequest request)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            RegisteredDate = DateTimeOffset.UtcNow
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            RegisteredDate = user.RegisteredDate
        };
    }

    public async Task<UserResponse?> GetByIdAsync(Guid id)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(user => user.Id == id)
            .Select(user => new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                RegisteredDate = user.RegisteredDate
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<UserResponse>> GetAllAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .Select(user => new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                RegisteredDate = user.RegisteredDate
            })
            .ToListAsync();
    }
}
