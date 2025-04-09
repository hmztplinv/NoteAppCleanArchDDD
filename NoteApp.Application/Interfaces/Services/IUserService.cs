using NoteApp.Domain.Entities;

namespace NoteApp.Application.Interfaces.Services;

public interface IUserService
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<List<string>> GetUserRolesAsync(User user);
    Task<bool> ValidateUserCredentialsAsync(string username, string password);
    Task<(bool success, string passwordHash, string passwordSalt)> CreatePasswordHashAsync(string password);
    Task<bool> VerifyPasswordHashAsync(string password, string storedHash, string storedSalt);
}