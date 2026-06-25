using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class ProvinceRepository
{
    private readonly PickleChicDbContext _context;

    public ProvinceRepository(PickleChicDbContext context)
    {
        _context = context;
    }

    public async Task<List<Province>> GetAllAsync()
    {
        return await _context.Provinces
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<Province?> GetByIdAsync(int id)
    {
        return await _context.Provinces
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Province> AddAsync(Province entity)
    {
        _context.Provinces.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Province?> UpdateAsync(Province entity)
    {
        try
        {
            var existing = await _context.Provinces.FindAsync(entity.Id);
            if (existing is null)
                return null;

            existing.Name = entity.Name;
            existing.Code = entity.Code;
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
        var entity = await _context.Provinces.FirstOrDefaultAsync(p => p.Id == id);
        if (entity is null)
            return false;

        _context.Provinces.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
