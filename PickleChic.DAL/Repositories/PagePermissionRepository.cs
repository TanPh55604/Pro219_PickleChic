using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class PagePermissionRepository
{
    private readonly PickleChicDbContext _context;

    public PagePermissionRepository(PickleChicDbContext context)
    {
        _context = context;
    }

    public async Task<List<PagePermission>> GetAllAsync()
    {
        return await _context.PagePermissions.ToListAsync();
    }

    public async Task<PagePermission?> GetByIdAsync(int id)
    {
        return await _context.PagePermissions.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PagePermission> AddAsync(PagePermission entity)
    {
        _context.PagePermissions.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<PagePermission?> UpdateAsync(PagePermission entity)
    {
        try
        {
            var existing = await _context.PagePermissions.FindAsync(entity.Id);
            if (existing is null)
                return null;

            existing.PageCode = entity.PageCode;
            existing.PageRoute = entity.PageRoute;
            existing.AvailablePermissions = entity.AvailablePermissions;
            existing.DefaultPermissions = entity.DefaultPermissions;

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
        var entity = await _context.PagePermissions.FirstOrDefaultAsync(p => p.Id == id);
        if (entity is null)
            return false;

        _context.PagePermissions.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
