using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class ProductVariantAttributeRepository
{
    private readonly PickleChicDbContext _context;

    public ProductVariantAttributeRepository(PickleChicDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductVariantAttribute>> GetAllAsync()
    {
        return await _context.ProductVariantAttributes
            .Where(pva => _context.ProductVariants.Any(pv =>
                pv.Id == pva.ProductVariantId
                && _context.Products.Any(p => p.Id == pv.ProductId && !p.IsDeleted)))
            .ToListAsync();
    }

    public async Task<ProductVariantAttribute?> GetByIdAsync(int id)
    {
        return await _context.ProductVariantAttributes
            .FirstOrDefaultAsync(pva => pva.Id == id
                && _context.ProductVariants.Any(pv =>
                    pv.Id == pva.ProductVariantId
                    && _context.Products.Any(p => p.Id == pv.ProductId && !p.IsDeleted)));
    }

    public async Task<ProductVariantAttribute> AddAsync(ProductVariantAttribute entity)
    {
        _context.ProductVariantAttributes.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<ProductVariantAttribute?> UpdateAsync(ProductVariantAttribute entity)
    {
        try
        {
            var existing = await _context.ProductVariantAttributes.FindAsync(entity.Id);
            if (existing is null)
                return null;

            existing.ProductVariantId = entity.ProductVariantId;
            existing.AttributeValueId = entity.AttributeValueId;
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
        var entity = await _context.ProductVariantAttributes.FindAsync(id);
        if (entity is null)
            return false;

        _context.ProductVariantAttributes.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
