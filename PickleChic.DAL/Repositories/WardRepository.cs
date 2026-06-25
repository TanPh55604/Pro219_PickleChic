using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class WardRepository
{
    private readonly PickleChicDbContext _context;

    public WardRepository(PickleChicDbContext context)
    {
        _context = context;
    }

    public async Task<List<Ward>> GetAllAsync()
    {
        return await _context.Wards
            .Include(w => w.District)
            .OrderBy(w => w.Name)
            .ToListAsync();
    }

    public async Task<List<Ward>> GetByDistrictIdAsync(int districtId)
    {
        return await _context.Wards
            .Include(w => w.District)
            .Where(w => w.DistrictId == districtId)
            .OrderBy(w => w.Name)
            .ToListAsync();
    }

    public async Task<Ward?> GetByIdAsync(int id)
    {
        return await _context.Wards
            .Include(w => w.District)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<Ward> AddAsync(Ward entity)
    {
        _context.Wards.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Ward?> UpdateAsync(Ward entity)
    {
        try
        {
            var existing = await _context.Wards.FindAsync(entity.Id);
            if (existing is null)
                return null;

            existing.Name = entity.Name;
            existing.Code = entity.Code;
            existing.DistrictId = entity.DistrictId;
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
        var entity = await _context.Wards.FirstOrDefaultAsync(w => w.Id == id);
        if (entity is null)
            return false;

        _context.Wards.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
