using System.Security.Cryptography;
using System.Text;
using NoteApp.Application.Interfaces.Repositories;
using NoteApp.Application.Interfaces.Services;
using NoteApp.Domain.Entities;

namespace NoteApp.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _userRepository.GetByIdAsync(id);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _userRepository.GetByUsernameAsync(username);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public async Task<List<string>> GetUserRolesAsync(User user)
    {
        return await _userRepository.GetUserRolesAsync(user.Id);
    }

    public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
        {
            return false;
        }

        return await VerifyPasswordHashAsync(password, user.PasswordHash, user.PasswordSalt);
    }

    public async Task<(bool success, string passwordHash, string passwordSalt)> CreatePasswordHashAsync(string password)
    {
        try
        {
            using var hmac = new HMACSHA512();
            var passwordSalt = Convert.ToBase64String(hmac.Key);
            var passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            
            return (true, passwordHash, passwordSalt);
        }
        catch
        {
            return (false, string.Empty, string.Empty);
        }
    }

    public async Task<bool> VerifyPasswordHashAsync(string password, string storedHash, string storedSalt)
    {
        try
        {
            var saltBytes = Convert.FromBase64String(storedSalt);
            using var hmac = new HMACSHA512(saltBytes);
            var computedHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            
            return computedHash == storedHash;
        }
        catch
        {
            return false;
        }
    }
}