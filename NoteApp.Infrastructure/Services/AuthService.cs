using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NoteApp.Application.Interfaces.Repositories;
using NoteApp.Application.Interfaces.Services;
using NoteApp.Application.Settings;
using NoteApp.Domain.Entities;

namespace NoteApp.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IUserRepository _userRepository;

    public AuthService(IOptions<JwtSettings> jwtSettings, IUserRepository userRepository)
    {
        _jwtSettings = jwtSettings.Value;
        _userRepository = userRepository;
    }

    public async Task<(string accessToken, string refreshToken)> GenerateTokensAsync(User user)
    {
        // Access token oluşturma
        var accessToken = await GenerateAccessTokenAsync(user);
        
        // Refresh token oluşturma
        var refreshToken = GenerateRefreshToken();
        
        // Refresh token'ı kullanıcıya kaydet
        var refreshTokenExpireDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenValidityInDays);
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpireDate = refreshTokenExpireDate;
        
        await _userRepository.UpdateAsync(user);
        
        return (accessToken, refreshToken);
    }

    public async Task<string?> ValidateRefreshTokenAsync(string refreshToken)
    {
        // Tüm kullanıcıları çekmek ideal değil, ancak örnek olarak bırakıyorum
        // Gerçek uygulamada direkt refresh token'a göre sorgu yapan bir repository metodu olmalı
        var users = await _userRepository.GetAllAsync();
        var user = users.FirstOrDefault(u => 
            u.RefreshToken == refreshToken && 
            u.RefreshTokenExpireDate > DateTime.UtcNow);
        
        return user?.Id.ToString();
    }

    public async Task<bool> RevokeRefreshTokenAsync(string userId)
    {
        if (!Guid.TryParse(userId, out var userGuid))
        {
            return false;
        }
        
        var user = await _userRepository.GetByIdAsync(userGuid);
        if (user == null)
        {
            return false;
        }
        
        user.RefreshToken = null;
        user.RefreshTokenExpireDate = null;
        
        await _userRepository.UpdateAsync(user);
        return true;
    }

    private async Task<string> GenerateAccessTokenAsync(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        // Kullanıcı rollerini al
        var roles = await _userRepository.GetUserRolesAsync(user.Id);
        
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Name, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };
        
        // Rolleri claim olarak ekle
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}