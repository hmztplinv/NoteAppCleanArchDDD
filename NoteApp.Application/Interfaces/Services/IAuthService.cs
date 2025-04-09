using NoteApp.Domain.Entities;

namespace NoteApp.Application.Interfaces.Services;

public interface IAuthService
{
    Task<(string accessToken, string refreshToken)> GenerateTokensAsync(User user);
    Task<string?> ValidateRefreshTokenAsync(string refreshToken);
    Task<bool> RevokeRefreshTokenAsync(string userId);
}