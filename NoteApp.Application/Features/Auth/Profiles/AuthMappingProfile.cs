using AutoMapper;
using NoteApp.Application.Features.Auth.Dtos;
using NoteApp.Domain.Entities;

namespace NoteApp.Application.Features.Auth.Profiles;

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<User, UserDto>();
    }
}