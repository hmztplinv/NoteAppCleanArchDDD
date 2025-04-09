using NoteApp.Domain.Common;

namespace NoteApp.Domain.Entities;

public class Role : BaseEntity
{
    public string Name { get; set; } = null!;
    public string NormalizedName { get; set; } = null!;
    public string? Description { get; set; }
    
    // Roller ile ilişkili kullanıcılar
    public List<UserRole> UserRoles { get; set; } = new();
}