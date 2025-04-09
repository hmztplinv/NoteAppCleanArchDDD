namespace NoteApp.Application.Features.Auth.Dtos;

public class AuthResponseDto
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime Expiration { get; set; }
    public string Username { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
}