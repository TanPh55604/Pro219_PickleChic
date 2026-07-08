using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class PromotionDetailRepository
{
    private readonly PickleChicDbContext _context;

    public PromotionDetailRepository(PickleChicDbContext context)
    {
        _context = context;
    }

    public async Task<List<PromotionDetail>> GetAllAsync()
    {
        return await _context.PromotionDetails
            .Where(pd => _context.ProductVariants.Any(pv =>
                pv.Id == pd.ProductVariantId
                && _context.Products.Any(p => p.Id == pv.ProductId && !p.IsDeleted)))
            .ToListAsync();
    }

    public async Task<PromotionDetail?> GetByIdAsync(int id)
    {
        return await _context.PromotionDetails
            .FirstOrDefaultAsync(pd => pd.Id == id
                && _context.ProductVariants.Any(pv =>
                    pv.Id == pd.ProductVariantId
                    && _context.Products.Any(p => p.Id == pv.ProductId && !p.IsDeleted)));
    }

    public async Task<PromotionDetail> AddAsync(PromotionDetail entity)
    {
        _context.PromotionDetails.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<PromotionDetail?> UpdateAsync(PromotionDetail entity)
    {
        try
        {
            var existing = await _context.PromotionDetails.FindAsync(entity.Id);
            if (existing is null)
                return null;

            existing.PromotionId = entity.PromotionId;
            existing.ProductVariantId = entity.ProductVariantId;
            existing.DiscountType = entity.DiscountType;
            existing.DiscountValue = entity.DiscountValue;
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
        var entity = await _context.PromotionDetails.FindAsync(id);
        if (entity is null)
            return false;

        _context.PromotionDetails.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
