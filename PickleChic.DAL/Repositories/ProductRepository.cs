using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class ProductRepository
{
    private readonly PickleChicDbContext _context;

    public ProductRepository(PickleChicDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products
            .Where(p => !p.IsDeleted
                && _context.Categories.Any(c => c.Id == p.CategoryId && !c.Delete)
                && _context.Brands.Any(b => b.Id == p.BrandId && !b.Delete))
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted
                && _context.Categories.Any(c => c.Id == p.CategoryId && !c.Delete)
                && _context.Brands.Any(b => b.Id == p.BrandId && !b.Delete));
    }

    public async Task<List<Product>> GetProductsWithDetailsAsync(string? keyword)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.ProductVariants!)
                .ThenInclude(pv => pv.ProductVariantImages)
            .Include(p => p.ProductVariants!)
                .ThenInclude(pv => pv.ProductVariantAttributes!)
                    .ThenInclude(pva => pva.AttributeValue)
                        .ThenInclude(av => av!.ProductAttribute)
            .Where(p => !p.IsDeleted
                && _context.Categories.Any(c => c.Id == p.CategoryId && !c.Delete)
                && _context.Brands.Any(b => b.Id == p.BrandId && !b.Delete));

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(p => p.ProductName.Contains(keyword));
        }

        return await query.ToListAsync();
    }

    public async Task<Product?> GetProductWithDetailsByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.ProductVariants!)
                .ThenInclude(pv => pv.ProductVariantImages)
            .Include(p => p.ProductVariants!)
                .ThenInclude(pv => pv.ProductVariantAttributes!)
                    .ThenInclude(pva => pva.AttributeValue)
                        .ThenInclude(av => av!.ProductAttribute)
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted
                && _context.Categories.Any(c => c.Id == p.CategoryId && !c.Delete)
                && _context.Brands.Any(b => b.Id == p.BrandId && !b.Delete));
    }


    public async Task<Product> AddAsync(Product entity)
    {
        _context.Products.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Product?> UpdateAsync(Product entity)
    {
        try
        {
            var existing = await _context.Products.FindAsync(entity.Id);
            if (existing is null)
                return null;
            if (existing.IsDeleted)
                return null;

            existing.ProductName = entity.ProductName;
            existing.Description = entity.Description;
            existing.CategoryId = entity.CategoryId;
            existing.BrandId = entity.BrandId;
            existing.Status = entity.Status;
            existing.UpdatedBy = entity.UpdatedBy;
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
        var entity = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (entity is null || entity.IsDeleted)
            return false;

        entity.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }
}
