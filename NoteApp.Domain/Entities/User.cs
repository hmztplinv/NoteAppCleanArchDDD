using NoteApp.Domain.Common;

namespace NoteApp.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string PasswordSalt { get; set; } = null!; 
    public bool IsActive { get; set; } = true;
    public bool EmailConfirmed { get; set; } = false;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpireDate { get; set; }
    
    // user roles 
    public List<UserRole> UserRoles { get; set; } = new();
    
    // Kullanıcının yazıları, yorumları ve değerlendirmeleri (İlişkiler)
    public List<Note> Notes { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
    public List<Rating> Ratings { get; set; } = new();
}