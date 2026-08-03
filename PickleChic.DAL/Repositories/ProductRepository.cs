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

    public async Task<bool> ExistsByNameAsync(string productName, int? excludeId = null)
    {
        var normalized = productName.Trim().ToLower();
        return await _context.Products.AnyAsync(p =>
            !p.IsDeleted
            && p.ProductName.ToLower() == normalized
            && (!excludeId.HasValue || p.Id != excludeId.Value));
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

    public async Task<List<Product>> SearchProductsWithVariantsAsync(
        string? keyword,
        decimal? startingPrice = null,
        decimal? toPrice = null,
        string? sortBy = null,
        int? pageNumber = null,
        int? pageSize = null)
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
            var lowerKeyword = keyword.ToLower();
            query = query.Where(p =>
                p.ProductName.ToLower().Contains(lowerKeyword)
                || (p.Description != null && p.Description.ToLower().Contains(lowerKeyword))
                || (p.Category != null && p.Category.Name.ToLower().Contains(lowerKeyword))
                || (p.Brand != null && p.Brand.Name.ToLower().Contains(lowerKeyword))
                || p.ProductVariants!.Any(pv =>
                    (pv.VariantName != null && pv.VariantName.ToLower().Contains(lowerKeyword))
                    || pv.SKU.ToLower().Contains(lowerKeyword)
                    || pv.ProductVariantAttributes!.Any(pva =>
                        pva.AttributeValue != null && (
                            pva.AttributeValue.Value.ToLower().Contains(lowerKeyword)
                            || (pva.AttributeValue.ProductAttribute != null && pva.AttributeValue.ProductAttribute.AttributeName.ToLower().Contains(lowerKeyword))
                        )
                    )
                )
            );
        }

        if (startingPrice.HasValue)
        {
            query = query.Where(p => p.ProductVariants!.Any(pv => pv.Price >= startingPrice.Value));
        }

        if (toPrice.HasValue)
        {
            query = query.Where(p => p.ProductVariants!.Any(pv => pv.Price <= toPrice.Value));
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            query = sortBy.ToLower() switch
            {
                "name_asc" => query.OrderBy(p => p.ProductName),
                "name_desc" => query.OrderByDescending(p => p.ProductName),
                "price_asc" => query.OrderBy(p => p.ProductVariants!.Min(pv => (decimal?)pv.Price) ?? 0),
                "price_desc" => query.OrderByDescending(p => p.ProductVariants!.Max(pv => (decimal?)pv.Price) ?? 0),
                _ => query
            };
        }

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<List<Product>> FilterProductsWithDetailsAsync(
        string? keyword,
        int? brandId,
        int? categoryId,
        int? attributeId,
        List<int>? attributeValueIds,
        decimal? startingPrice = null,
        decimal? toPrice = null,
        string? sortBy = null)
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
                && p.Status == 1
                && _context.Categories.Any(c => c.Id == p.CategoryId && !c.Delete && c.Status == 1)
                && _context.Brands.Any(b => b.Id == p.BrandId && !b.Delete && b.Status == 1));

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var lowerKeyword = keyword.ToLower();
            query = query.Where(p =>
                p.ProductName.ToLower().Contains(lowerKeyword)
                || (p.ProductVariants != null && p.ProductVariants.Any(pv =>
                    pv.SKU.ToLower().Contains(lowerKeyword)
                    || (pv.VariantName != null && pv.VariantName.ToLower().Contains(lowerKeyword))
                ))
            );
        }

        if (brandId.HasValue)
        {
            query = query.Where(p => p.BrandId == brandId.Value);
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (attributeId.HasValue)
        {
            query = query.Where(p => p.ProductVariants != null && p.ProductVariants.Any(pv =>
                pv.ProductVariantAttributes != null && pv.ProductVariantAttributes.Any(pva =>
                    pva.AttributeValue != null && pva.AttributeValue.AttributeId == attributeId.Value
                )
            ));
        }

        List<List<int>>? attributeValueGroups = null;

        if (attributeValueIds != null && attributeValueIds.Any())
        {
            attributeValueGroups = await GroupAttributeValueIdsByAttributeAsync(attributeValueIds);

            // Coarse SQL filter: keep products that match at least one selected value.
            query = query.Where(p => p.ProductVariants != null && p.ProductVariants.Any(pv =>
                pv.ProductVariantAttributes != null && pv.ProductVariantAttributes.Any(pva =>
                    attributeValueIds.Contains(pva.AttributeValueId)
                )
            ));
        }

        if (startingPrice.HasValue)
        {
            query = query.Where(p => p.ProductVariants != null
                && p.ProductVariants.Any(pv => pv.Price >= startingPrice.Value));
        }

        if (toPrice.HasValue)
        {
            query = query.Where(p => p.ProductVariants != null
                && p.ProductVariants.Any(pv => pv.Price <= toPrice.Value));
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            query = sortBy.Trim().ToLowerInvariant() switch
            {
                "name_asc" => query.OrderBy(p => p.ProductName),
                "name_desc" => query.OrderByDescending(p => p.ProductName),
                "price_asc" => query.OrderBy(p => p.ProductVariants!.Min(pv => (decimal?)pv.Price) ?? 0),
                "price_desc" => query.OrderByDescending(p => p.ProductVariants!.Max(pv => (decimal?)pv.Price) ?? 0),
                _ => query.OrderBy(p => p.ProductName)
            };
        }
        else
        {
            query = query.OrderBy(p => p.ProductName);
        }

        var products = await query.ToListAsync();

        if (attributeValueGroups is { Count: > 0 })
        {
            products = products
                .Where(p => p.ProductVariants != null
                    && p.ProductVariants.Any(pv => VariantMatchesAttributeGroups(pv, attributeValueGroups)))
                .ToList();
        }

        return products;
    }

    public async Task<List<List<int>>> GroupAttributeValueIdsByAttributeAsync(IEnumerable<int> attributeValueIds)
    {
        var ids = attributeValueIds
            .Where(id => id > 0)
            .Distinct()
            .ToList();

        if (ids.Count == 0)
        {
            return new List<List<int>>();
        }

        return await _context.AttributeValues
            .AsNoTracking()
            .Where(av => ids.Contains(av.Id))
            .GroupBy(av => av.AttributeId)
            .Select(g => g.Select(av => av.Id).ToList())
            .ToListAsync();
    }

    public static bool VariantMatchesAttributeGroups(
        ProductVariant? variant,
        IReadOnlyList<IReadOnlyList<int>> attributeValueGroups)
    {
        if (attributeValueGroups.Count == 0)
        {
            return true;
        }

        if (variant?.ProductVariantAttributes is null || variant.ProductVariantAttributes.Count == 0)
        {
            return false;
        }

        var valueIds = variant.ProductVariantAttributes
            .Select(pva => pva.AttributeValueId)
            .ToHashSet();

        return attributeValueGroups.All(group => group.Any(id => valueIds.Contains(id)));
    }
}

