using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class CategoryRepository
{
    private readonly PickleChicDbContext _context;

    public CategoryRepository(PickleChicDbContext context)
    {
        _context = context;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _context.Categories
            .Where(c => !c.Delete)
            .ToListAsync();
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id && !c.Delete);
    }

    public async Task<Category> AddAsync(Category entity)
    {
        _context.Categories.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Category?> UpdateAsync(Category entity)
    {
        try
        {
            var existing = await _context.Categories.FindAsync(entity.Id);
            if (existing is null)
                return null;
            if (existing.Delete)
                return null;

            existing.Name = entity.Name;
            existing.Description = entity.Description;
            existing.Status = entity.Status;
            existing.UpdatedAt = entity.UpdatedAt ?? DateTime.Now;
            await _context.SaveChangesAsync();
            return existing;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<Category?> UpdateLinkImageAsync(int id, string? linkImage)
    {
        var existing = await _context.Categories.FindAsync(id);
        if (existing is null || existing.Delete)
        {
            return null;
        }

        existing.LinkImage = linkImage;
        existing.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (entity is null)
            return false;

        _context.Categories.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var entity = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        if (entity is null || entity.Delete)
            return false;

        entity.Delete = true;
        await _context.SaveChangesAsync();
        return true;
    }
}
