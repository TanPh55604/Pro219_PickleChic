using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class VoucherRepository
{
    private readonly PickleChicDbContext _context;

    public VoucherRepository(PickleChicDbContext context)
    {
        _context = context;
    }

    public async Task<List<Voucher>> GetAllAsync()
    {
        return await _context.Vouchers.ToListAsync();
    }

    public async Task<Voucher?> GetByIdAsync(int id)
    {
        return await _context.Vouchers.FindAsync(id);
    }

    public async Task<Voucher?> GetByCodeAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        var normalized = code.Trim();

        return await _context.Vouchers
            .FirstOrDefaultAsync(x => x.VoucherCode.ToLower() == normalized.ToLower());
    }

    public async Task<List<Voucher>> GetAvailableByMinSpend(decimal minSpend)
    {
        return await _context.Vouchers.Where(x => (x.MinimumSpend == null || x.MinimumSpend <= minSpend) && x.IsActive).ToListAsync();
    }




    public async Task<Voucher> AddAsync(Voucher entity)
    {
        _context.Vouchers.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Voucher?> UpdateAsync(Voucher entity)
    {
        try
        {
            var existing = await _context.Vouchers.FindAsync(entity.Id);
            if (existing is null)
                return null;

            existing.VoucherCode = entity.VoucherCode;
            existing.DiscountType = entity.DiscountType;
            existing.DiscountValue = entity.DiscountValue;
            existing.MinOrderValue = entity.MinOrderValue;
            existing.MaxDiscountAmount = entity.MaxDiscountAmount;
            existing.MinimumSpend = entity.MinimumSpend;
            existing.StartDate = entity.StartDate;
            existing.EndDate = entity.EndDate;
            existing.UsageLimit = entity.UsageLimit;
            existing.CustomerUsageLimit = entity.CustomerUsageLimit;
            existing.UsedCount = entity.UsedCount;
            existing.IsActive = entity.IsActive;
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
        var entity = await _context.Vouchers.FindAsync(id);
        if (entity is null)
            return false;

        _context.Vouchers.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
