using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class DistrictRepository
{
    private readonly PickleChicDbContext _context;

    public DistrictRepository(PickleChicDbContext context)
    {
        _context = context;
    }

    public async Task<List<District>> GetAllAsync()
    {
        return await _context.Districts
            .Include(d => d.Province)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<List<District>> GetByProvinceIdAsync(int provinceId)
    {
        return await _context.Districts
            .Include(d => d.Province)
            .Where(d => d.ProvinceId == provinceId)
            .OrderBy(d => d.Name)
            .ToListAsync();
    }

    public async Task<District?> GetByIdAsync(int id)
    {
        return await _context.Districts
            .Include(d => d.Province)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<District> AddAsync(District entity)
    {
        _context.Districts.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<District?> UpdateAsync(District entity)
    {
        try
        {
            var existing = await _context.Districts.FindAsync(entity.Id);
            if (existing is null)
                return null;

            existing.Name = entity.Name;
            existing.Code = entity.Code;
            existing.ProvinceId = entity.ProvinceId;
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
        var entity = await _context.Districts.FirstOrDefaultAsync(d => d.Id == id);
        if (entity is null)
            return false;

        _context.Districts.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
