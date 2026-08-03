using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class RoleRepository
{
    private readonly PickleChicDbContext _context;

    public RoleRepository(PickleChicDbContext context)
    {
        _context = context;
    }

    public async Task<List<Role>> GetAllAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<Role?> GetByIdAsync(int id)
    {
        return await _context.Roles.FindAsync(id);
    }

    public async Task<Role> AddAsync(Role entity)
    {
        _context.Roles.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Role?> UpdateAsync(Role entity)
    {
        try
        {
            var existing = await _context.Roles.FindAsync(entity.Id);
            if (existing is null)
                return null;

            existing.RoleName = entity.RoleName;
            existing.Permissions = entity.Permissions;
            existing.Status = entity.Status;
            await _context.SaveChangesAsync();
            return existing;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Roles.FindAsync(id);
        if (entity is null)
            return false;

        _context.Roles.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
