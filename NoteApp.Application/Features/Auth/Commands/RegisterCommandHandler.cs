using MediatR;
using NoteApp.Application.Interfaces.Repositories;
using NoteApp.Application.Interfaces.Services;
using NoteApp.Application.Wrappers;
using NoteApp.Domain.Entities;

namespace NoteApp.Application.Features.Auth.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ApiResponse<Guid>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserService _userService;

    public RegisterCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserService userService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userService = userService;
    }

    public async Task<ApiResponse<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Şifre hash'leme
        var (success, passwordHash, passwordSalt) = await _userService.CreatePasswordHashAsync(request.Password);
        
        if (!success)
        {
            return new ApiResponse<Guid>("Şifre hashleme işlemi sırasında bir hata oluştu.");
        }

        // Yeni kullanıcı oluşturma
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            IsActive = true,
            EmailConfirmed = false // E-posta onay sistemi eklenebilir
        };

        await _userRepository.AddAsync(user);

        // Kullanıcıya varsayılan "User" rolünü atama
        var userRole = await _roleRepository.GetByNameAsync("User");
        if (userRole != null)
        {
            await _userRepository.AssignRoleToUserAsync(user.Id, userRole.Id);
        }

        return new ApiResponse<Guid>(user.Id);
    }
}