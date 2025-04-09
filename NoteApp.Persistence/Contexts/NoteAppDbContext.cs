using Microsoft.EntityFrameworkCore;
using NoteApp.Application.Interfaces.Services;
using NoteApp.Domain.Common;
using NoteApp.Domain.Entities;

namespace NoteApp.Persistence.Contexts;

// EF Core DbContext implementation
public class NoteAppDbContext : DbContext
{
    private readonly ICurrentUserService? _currentUserService;

    public NoteAppDbContext(
        DbContextOptions<NoteAppDbContext> options,
        ICurrentUserService? currentUserService = null) : base(options)
    {
        _currentUserService = currentUserService;
    }

    public DbSet<Note> Notes => Set<Note>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Rating> Ratings => Set<Rating>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NoteAppDbContext).Assembly);
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = DateTime.UtcNow;
                    entry.Entity.CreatedBy = _currentUserService?.Username;
                    break;
                case EntityState.Modified:
                    entry.Entity.ModifiedDate = DateTime.UtcNow;
                    entry.Entity.ModifiedBy = _currentUserService?.Username;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}