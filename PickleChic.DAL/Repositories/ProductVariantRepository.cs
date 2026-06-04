using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class ProductVariantRepository
{
    private readonly PickleChicDbContext _context;

    public ProductVariantRepository(PickleChicDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductVariant>> GetAllAsync()
    {
        return await _context.ProductVariants
            .Where(pv => _context.Products.Any(p => p.Id == pv.ProductId && !p.IsDeleted))
            .ToListAsync();
    }

    public async Task<ProductVariant?> GetByIdAsync(int id)
    {
        return await _context.ProductVariants
            .FirstOrDefaultAsync(pv => pv.Id == id
                && _context.Products.Any(p => p.Id == pv.ProductId && !p.IsDeleted));
    }

    public async Task<ProductVariant?> GetVariantWithDetailsByIdAsync(int id)
    {
        return await _context.ProductVariants
            .Include(pv => pv.ProductVariantImages)
            .Include(pv => pv.ProductVariantAttributes!)
                .ThenInclude(pva => pva.AttributeValue)
                    .ThenInclude(av => av!.ProductAttribute)
            .FirstOrDefaultAsync(pv => pv.Id == id
                && _context.Products.Any(p => p.Id == pv.ProductId && !p.IsDeleted));
    }


    public async Task<ProductVariant> AddAsync(ProductVariant entity)
    {
        _context.ProductVariants.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<ProductVariant?> UpdateAsync(ProductVariant entity)
    {
        try
        {
            var existing = await _context.ProductVariants.FindAsync(entity.Id);
            if (existing is null)
                return null;

            existing.ProductId = entity.ProductId;
            existing.SKU = entity.SKU;
            existing.VariantName = entity.VariantName;
            existing.Price = entity.Price;
            existing.StockQuantity = entity.StockQuantity;
            existing.Status = entity.Status;
            await _context.SaveChangesAsync();
            return existing;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<ProductVariant?> UpdateWithAttributesAsync(ProductVariant entity, List<int> attributeValueIds)
    {
        try
        {
            var existing = await _context.ProductVariants
                .Include(pv => pv.ProductVariantAttributes)
                .FirstOrDefaultAsync(pv => pv.Id == entity.Id);
            if (existing is null)
                return null;

            existing.ProductId = entity.ProductId;
            existing.SKU = entity.SKU;
            existing.VariantName = entity.VariantName;
            existing.Price = entity.Price;
            existing.StockQuantity = entity.StockQuantity;
            existing.Status = entity.Status;

            if (attributeValueIds != null)
            {
                // Remove attributes no longer present
                var toRemove = existing.ProductVariantAttributes?
                    .Where(pva => !attributeValueIds.Contains(pva.AttributeValueId))
                    .ToList();
                if (toRemove != null)
                {
                    _context.ProductVariantAttributes.RemoveRange(toRemove);
                }

                // Add new attributes
                var existingIds = existing.ProductVariantAttributes?.Select(pva => pva.AttributeValueId).ToList() ?? new List<int>();
                var toAdd = attributeValueIds
                    .Where(id => !existingIds.Contains(id))
                    .Select(id => new ProductVariantAttribute
                    {
                        ProductVariantId = entity.Id,
                        AttributeValueId = id
                    });

                foreach (var attr in toAdd)
                {
                    _context.ProductVariantAttributes.Add(attr);
                }
            }

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
        var entity = await _context.ProductVariants.FindAsync(id);
        if (entity is null)
            return false;

        _context.ProductVariants.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
