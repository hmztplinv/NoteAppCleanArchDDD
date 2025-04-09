using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NoteApp.Domain.Entities;

namespace NoteApp.Persistence.Configurations;

// Rating entity configuration
public class RatingConfiguration : IEntityTypeConfiguration<Rating>
{
    public void Configure(EntityTypeBuilder<Rating> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Score).IsRequired();
        
        // Not ilişkisi
        builder.HasOne(r => r.Note)
               .WithMany(n => n.Ratings)
               .HasForeignKey(r => r.NoteId);
               
        // Kullanıcı ilişkisi UserConfiguration'da tanımlandı
               
        // Bir kullanıcı bir notu sadece bir kez değerlendirebilir
        builder.HasIndex(r => new { r.NoteId, r.UserId })
               .IsUnique();
    }
}