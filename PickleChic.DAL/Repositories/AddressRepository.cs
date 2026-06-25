using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class AddressRepository
{
    private readonly PickleChicDbContext _context;

    public AddressRepository(PickleChicDbContext context)
    {
        _context = context;
    }

    public async Task<List<Address>> GetAllAsync()
    {
        return await _context.Addresses
            .Include(a => a.Ward)
                .ThenInclude(w => w.District)
                    .ThenInclude(d => d.Province)
            .Where(a => !a.Delete)
            .ToListAsync();
    }

    public async Task<Address?> GetByIdAsync(int id)
    {
        return await _context.Addresses
            .Include(a => a.Ward)
                .ThenInclude(w => w.District)
                    .ThenInclude(d => d.Province)
            .FirstOrDefaultAsync(a => a.Id == id && !a.Delete);
    }

    public async Task<Address> AddAsync(Address entity)
    {
        _context.Addresses.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Address?> UpdateAsync(Address entity)
    {
        try
        {
            var existing = await _context.Addresses.FindAsync(entity.Id);
            if (existing is null)
                return null;
            if (existing.Delete)
                return null;

            existing.CustomerId = entity.CustomerId;
            existing.FullName = entity.FullName;
            existing.PhoneNumber = entity.PhoneNumber;
            existing.WardId = entity.WardId;
            existing.DetailInfo = entity.DetailInfo;
            existing.IsDefault = entity.IsDefault;
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
        var entity = await _context.Addresses.FirstOrDefaultAsync(a => a.Id == id);
        if (entity is null || entity.Delete)
            return false;

        entity.Delete = true;
        await _context.SaveChangesAsync();
        return true;
    }
}
