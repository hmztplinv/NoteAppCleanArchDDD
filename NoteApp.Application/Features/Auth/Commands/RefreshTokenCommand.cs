using MediatR;
using NoteApp.Application.Features.Auth.Dtos;
using NoteApp.Application.Wrappers;

namespace NoteApp.Application.Features.Auth.Commands;

public class RefreshTokenCommand : IRequest<ApiResponse<AuthResponseDto>>
{
    public string RefreshToken { get; set; } = null!;
}