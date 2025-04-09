using System.IdentityModel.Tokens.Jwt;
using MediatR;
using NoteApp.Application.Features.Auth.Dtos;
using NoteApp.Application.Interfaces.Repositories;
using NoteApp.Application.Interfaces.Services;
using NoteApp.Application.Wrappers;

namespace NoteApp.Application.Features.Auth.Commands;

public class LoginCommandHandler : IRequestHandler<LoginCommand, ApiResponse<AuthResponseDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;
    private readonly IAuthService _authService;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IUserService userService,
        IAuthService authService)
    {
        _userRepository = userRepository;
        _userService = userService;
        _authService = authService;
    }

    public async Task<ApiResponse<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Kullanıcı adı kontrolü
        var user = await _userRepository.GetByUsernameAsync(request.Username);
        if (user == null)
        {
            return new ApiResponse<AuthResponseDto>("Kullanıcı adı veya şifre hatalı.");
        }

        // Kullanıcı aktif değilse
        if (!user.IsActive)
        {
            return new ApiResponse<AuthResponseDto>("Hesabınız aktif değil. Lütfen yönetici ile iletişime geçiniz.");
        }

        // Şifre doğrulama
        var isPasswordValid = await _userService.VerifyPasswordHashAsync(
            request.Password, user.PasswordHash, user.PasswordSalt);

        if (!isPasswordValid)
        {
            return new ApiResponse<AuthResponseDto>("Kullanıcı adı veya şifre hatalı.");
        }

        // Token oluşturma
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