using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoteApp.Domain.Entities;

namespace NoteApp.Persistence.Configurations;

// Comment entity configuration
public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Text).IsRequired();
        
        // Not ilişkisi
        builder.HasOne(c => c.Note)
               .WithMany(n => n.Comments)
               .HasForeignKey(c => c.NoteId);
               
        // Kullanıcı ilişkisi UserConfiguration'da tanımlandı
    }
}