using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class PaymentMethodRepository
{
    private readonly PickleChicDbContext _context;

    public PaymentMethodRepository(PickleChicDbContext context)
    {
        _context = context;
    }

    public async Task<List<PaymentMethod>> GetAllAsync()
    {
        return await _context.PaymentMethods
            .Where(p => !p.Delete)
            .ToListAsync();
    }

    public async Task<PaymentMethod?> GetByIdAsync(int id)
    {
        return await _context.PaymentMethods
            .FirstOrDefaultAsync(p => p.Id == id && !p.Delete);
    }

    public async Task<PaymentMethod> AddAsync(PaymentMethod entity)
    {
        _context.PaymentMethods.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<PaymentMethod?> UpdateAsync(PaymentMethod entity)
    {
        try
        {
            var existing = await _context.PaymentMethods.FindAsync(entity.Id);
            if (existing is null)
                return null;
            if (existing.Delete)
                return null;

            existing.Name = entity.Name;
            existing.Description = entity.Description;
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
        var entity = await _context.PaymentMethods.FirstOrDefaultAsync(p => p.Id == id);
        if (entity is null || entity.Delete)
            return false;

        entity.Delete = true;
        await _context.SaveChangesAsync();
        return true;
    }
}
