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

    public async Task<List<Address>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.Addresses
            .Include(a => a.Ward)
                .ThenInclude(w => w.District)
                    .ThenInclude(d => d.Province)
            .Where(a => a.CustomerId == customerId && !a.Delete && a.Status == 1)
            .ToListAsync();
    }

    public async Task<Address?> FindPickupAddressAsync(int customerId)
    {
        return await _context.Addresses
            .FirstOrDefaultAsync(a =>
                a.CustomerId == customerId
                && !a.Delete
                && (a.Status == 0 || a.DetailInfo == "Mua tại quầy"));
    }

    public async Task<Address> EnsureSystemPickupAsync(int customerId)
    {
        var dummy = await FindPickupAddressAsync(customerId);
        if (dummy != null)
        {
            if (dummy.Status != 0 || dummy.IsDefault)
            {
                dummy.Status = 0;
                dummy.IsDefault = false;
                dummy.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }

            return dummy;
        }

        dummy = new Address
        {
            CustomerId = customerId,
            FullName = customerId > 0 ? "Nhận tại quầy" : "Khách vãng lai",
            PhoneNumber = "0000000000",
            WardId = 1,
            DetailInfo = "Mua tại quầy",
            IsDefault = false,
            Status = 0,
            InsertedAt = DateTime.Now,
            Delete = false
        };

        return await AddAsync(dummy);
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
            if (existing.Status == 0)
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
        if (entity.Status == 0)
            return false;

        entity.Delete = true;
        await _context.SaveChangesAsync();
        return true;
    }
}
