namespace NoteApp.Application.Settings;

public class JwtSettings
{
    public string Secret { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int ExpiryMinutes { get; set; }
    public int RefreshTokenValidityInDays { get; set; }
}