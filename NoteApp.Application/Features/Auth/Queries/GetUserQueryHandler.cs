using AutoMapper;
using MediatR;
using NoteApp.Application.Features.Auth.Dtos;
using NoteApp.Application.Interfaces.Repositories;
using NoteApp.Application.Wrappers;

namespace NoteApp.Application.Features.Auth.Queries;

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, ApiResponse<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public GetUserQueryHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            return new ApiResponse<UserDto>("Kullanıcı bulunamadı.");
        }

        var roles = await _userRepository.GetUserRolesAsync(user.Id);
        var userDto = _mapper.Map<UserDto>(user);
        userDto.Roles = roles;

        return new ApiResponse<UserDto>(userDto);
    }
}