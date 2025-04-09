using NoteApp.Domain.Common;

namespace NoteApp.Domain.Entities;

public class Rating : BaseEntity
{
    public Guid NoteId { get; set; }
    public Note? Note { get; set; }
    public int Score { get; set; } // from 1 to 5
    public Guid UserId { get; set; }
    public User? User { get; set; }
}
