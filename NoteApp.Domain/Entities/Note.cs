using NoteApp.Domain.Common;

namespace NoteApp.Domain.Entities;

public class Note : BaseEntity
{
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public Guid CategoryId { get; set; }
    public Category? Category { get; set; }
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public List<Comment> Comments { get; set; } = new();
    public List<Rating> Ratings { get; set; } = new();
    public bool IsPublished { get; set; } = false;
}
