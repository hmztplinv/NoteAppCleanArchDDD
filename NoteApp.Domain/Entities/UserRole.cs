using NoteApp.Domain.Common;

namespace NoteApp.Domain.Entities;

// Çoka-çok ilişki tablosu
public class UserRole : BaseEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    
    public Guid RoleId { get; set; }
    public Role? Role { get; set; }
}