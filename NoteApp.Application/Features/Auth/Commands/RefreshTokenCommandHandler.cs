using System.IdentityModel.Tokens.Jwt;
using MediatR;
using NoteApp.Application.Features.Auth.Dtos;
using NoteApp.Application.Interfaces.Repositories;
using NoteApp.Application.Interfaces.Services;
using NoteApp.Application.Wrappers;

namespace NoteApp.Application.Features.Auth.Commands;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<AuthResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthService _authService;

    public RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IAuthService authService)
    {
        _userRepository = userRepository;
        _authService = authService;
    }

    public async Task<ApiResponse<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Refresh token doğrulama
        var userId = await _authService.ValidateRefreshTokenAsync(request.RefreshToken);
        if (userId == null)
        {
            return new ApiResponse<AuthResponseDto>("Geçersiz veya süresi dolmuş refresh token.");
        }

        // Kullanıcıyı bulma
        var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
        if (user == null)
        {
            return new ApiResponse<AuthResponseDto>("Kullanıcı bulunamadı.");
        }

        // Yeni token oluşturma
        var (accessToken, refreshToken) = await _authService.GenerateTokensAsync(user);

        // Kullanıcı rollerini getirme
        var roles = await _userRepository.GetUserRolesAsync(user.Id);

        // JWT token expiration bilgisini çözme
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(accessToken);
        var expiration = jwtToken.ValidTo;

        var response = new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Expiration = expiration,
            Username = user.Username,
            Roles = roles
        };

        return new ApiResponse<AuthResponseDto>(response);
    }
}