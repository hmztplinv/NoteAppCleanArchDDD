using NoteApp.Domain.Common;

namespace NoteApp.Domain.Entities;

public class Comment : BaseEntity
{
    public Guid NoteId { get; set; }
    public Note? Note { get; set; }
    public string Text { get; set; } = null!;
    public Guid UserId { get; set; }
    public User? User { get; set; }
}
