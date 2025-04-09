using System.Security.Claims;

namespace NoteApp.Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(string username, string role);
}
