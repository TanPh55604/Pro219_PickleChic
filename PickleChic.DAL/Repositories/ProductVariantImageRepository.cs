using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class ProductVariantImageRepository
{
    private readonly PickleChicDbContext _context;

    public ProductVariantImageRepository(PickleChicDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductVariantImage>> GetAllAsync()
    {
        return await _context.ProductVariantImages
            .Where(img => _context.ProductVariants.Any(pv =>
                pv.Id == img.ProductVariantId
                && _context.Products.Any(p => p.Id == pv.ProductId && !p.IsDeleted)))
            .ToListAsync();
    }

    public async Task<ProductVariantImage?> GetByIdAsync(int id)
    {
        return await _context.ProductVariantImages
            .FirstOrDefaultAsync(img => img.Id == id
                && _context.ProductVariants.Any(pv =>
                    pv.Id == img.ProductVariantId
                    && _context.Products.Any(p => p.Id == pv.ProductId && !p.IsDeleted)));
    }

    public async Task<List<ProductVariantImage>> GetByVariantIdAsync(int variantId)
    {
        return await _context.ProductVariantImages
            .Where(img => img.ProductVariantId == variantId)
            .OrderByDescending(img => img.IsMain)
            .ThenBy(img => img.Id)
            .ToListAsync();
    }

    public async Task<int> CountByVariantIdAsync(int variantId)
    {
        return await _context.ProductVariantImages.CountAsync(img => img.ProductVariantId == variantId);
    }

    public async Task SetMainAsync(int imageId, int variantId)
    {
        var images = await _context.ProductVariantImages
            .Where(img => img.ProductVariantId == variantId)
            .ToListAsync();

        foreach (var image in images)
        {
            image.IsMain = image.Id == imageId;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<ProductVariantImage> AddAsync(ProductVariantImage entity)
    {
        _context.ProductVariantImages.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<ProductVariantImage?> UpdateAsync(ProductVariantImage entity)
    {
        try
        {
            var existing = await _context.ProductVariantImages.FindAsync(entity.Id);
            if (existing is null)
                return null;

            existing.ProductVariantId = entity.ProductVariantId;
            existing.URL = entity.URL;
            existing.Name = entity.Name;
            existing.Description = entity.Description;
            existing.IsMain = entity.IsMain;
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
        var entity = await _context.ProductVariantImages.FindAsync(id);
        if (entity is null)
            return false;

        _context.ProductVariantImages.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
