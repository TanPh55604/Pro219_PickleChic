using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class BrandRepository
{
    private readonly PickleChicDbContext _context;

    public BrandRepository(PickleChicDbContext context)
    {
        _context = context;
    }

    public async Task<List<Brand>> GetAllAsync()
    {
        return await _context.Brands
            .Where(b => !b.Delete)
            .ToListAsync();
    }

    public async Task<Brand?> GetByIdAsync(int id)
    {
        return await _context.Brands
            .FirstOrDefaultAsync(b => b.Id == id && !b.Delete);
    }

    public async Task<Brand> AddAsync(Brand entity)
    {
        _context.Brands.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Brand?> UpdateAsync(Brand entity)
    {
        try
        {
            var existing = await _context.Brands.FindAsync(entity.Id);
            if (existing is null)
                return null;
            if (existing.Delete)
                return null;

            existing.Name = entity.Name;
            existing.Description = entity.Description;
            existing.UpdateBy = entity.UpdateBy;
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

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id);
        if (entity is null)
            return false;

        _context.Brands.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var entity = await _context.Brands.FirstOrDefaultAsync(b => b.Id == id);
        if (entity is null || entity.Delete)
            return false;

        entity.Delete = true;
        await _context.SaveChangesAsync();
        return true;
    }
}
