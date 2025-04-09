using MediatR;
using NoteApp.Application.Features.Auth.Dtos;
using NoteApp.Application.Wrappers;

namespace NoteApp.Application.Features.Auth.Queries;

public class GetUserQuery : IRequest<ApiResponse<UserDto>>
{
    public Guid UserId { get; set; }
}