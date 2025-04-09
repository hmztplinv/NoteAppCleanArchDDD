using Microsoft.EntityFrameworkCore;
using NoteApp.Application.Interfaces.Repositories;
using NoteApp.Domain.Entities;
using NoteApp.Persistence.Contexts;

namespace NoteApp.Persistence.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly NoteAppDbContext _context;

    public RoleRepository(NoteAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Role>> GetAllAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<Role?> GetByIdAsync(Guid id)
    {
        return await _context.Roles.FindAsync(id);
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.NormalizedName == name.ToUpper());
    }

    public async Task AddAsync(Role role)
    {
        role.NormalizedName = role.Name.ToUpper();
        await _context.Roles.AddAsync(role);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Role role)
    {
        role.NormalizedName = role.Name.ToUpper();
        _context.Roles.Update(role);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Role role)
    {
        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
    }
}