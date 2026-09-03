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
            .Include(pv => pv.Product)
                .ThenInclude(p => p!.Category)
            .Include(pv => pv.Product)
                .ThenInclude(p => p!.Brand)
            .Include(pv => pv.PromotionDetails!)
                .ThenInclude(pd => pd.Promotion)
            .FirstOrDefaultAsync(pv => pv.Id == id
                && _context.Products.Any(p => p.Id == pv.ProductId && !p.IsDeleted));
    }


    public async Task<ProductVariant> AddAsync(ProductVariant entity)
    {
        _context.ProductVariants.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> ExistsBySkuAsync(string sku, int? excludeId = null)
    {
        var normalized = sku.Trim().ToLower();
        return await _context.ProductVariants.AnyAsync(pv =>
            pv.SKU.ToLower() == normalized
            && (!excludeId.HasValue || pv.Id != excludeId.Value));
    }

    public async Task<bool> ExistsByVariantNameAsync(int productId, string variantName, int? excludeId = null)
    {
        if (string.IsNullOrWhiteSpace(variantName))
        {
            return false;
        }

        var normalized = variantName.Trim().ToLower();
        return await _context.ProductVariants.AnyAsync(pv =>
            pv.ProductId == productId
            && pv.VariantName != null
            && pv.VariantName.ToLower() == normalized
            && (!excludeId.HasValue || pv.Id != excludeId.Value));
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
                var toRemove = existing.ProductVariantAttributes?
                    .Where(pva => !attributeValueIds.Contains(pva.AttributeValueId))
                    .ToList();
                if (toRemove != null)
                {
                    _context.ProductVariantAttributes.RemoveRange(toRemove);
                }

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

    private IQueryable<ProductVariant> ApplyFiltersAndSorting(
        IQueryable<ProductVariant> query,
        decimal? startingPrice,
        decimal? toPrice,
        string? sortBy)
    {
        if (startingPrice.HasValue)
        {
            query = query.Where(pv => pv.Price >= startingPrice.Value);
        }
        if (toPrice.HasValue)
        {
            query = query.Where(pv => pv.Price <= toPrice.Value);
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
        {
            query = sortBy.ToLower() switch
            {
                "name_asc" => query.OrderBy(pv => pv.Product!.ProductName).ThenBy(pv => pv.VariantName),
                "name_desc" => query.OrderByDescending(pv => pv.Product!.ProductName).ThenByDescending(pv => pv.VariantName),
                "price_asc" => query.OrderBy(pv => pv.Price),
                "price_desc" => query.OrderByDescending(pv => pv.Price),
                _ => query
            };
        }

        return query;
    }

    public async Task<PagedResult<ProductVariant>> SearchVariantsPagedAsync(
        string? keyword,
        decimal? startingPrice,
        decimal? toPrice,
        string? sortBy,
        int pageNumber,
        int pageSize)
    {
        if (pageNumber < 1)
        {
            pageNumber = 1;
        }

        if (pageSize < 1)
        {
            pageSize = 1;
        }

        var query = _context.ProductVariants
            .Include(pv => pv.ProductVariantImages)
            .Include(pv => pv.ProductVariantAttributes!)
                .ThenInclude(pva => pva.AttributeValue)
                    .ThenInclude(av => av!.ProductAttribute)
            .Include(pv => pv.Product)
                .ThenInclude(p => p!.Category)
            .Include(pv => pv.Product)
                .ThenInclude(p => p!.Brand)
            .Where(pv => pv.Product != null && !pv.Product.IsDeleted && pv.Status > 0);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var lowerKeyword = keyword.ToLower();
            query = query.Where(pv =>
                (pv.VariantName != null && pv.VariantName.ToLower().Contains(lowerKeyword))
                || pv.SKU.ToLower().Contains(lowerKeyword)
                || pv.Product!.ProductName.ToLower().Contains(lowerKeyword)
                || (pv.Product.Category != null && pv.Product.Category.Name.ToLower().Contains(lowerKeyword))
                || (pv.Product.Brand != null && pv.Product.Brand.Name.ToLower().Contains(lowerKeyword))
                || pv.ProductVariantAttributes!.Any(pva =>
                    pva.AttributeValue != null && (
                        pva.AttributeValue.Value.ToLower().Contains(lowerKeyword)
                        || (pva.AttributeValue.ProductAttribute != null && pva.AttributeValue.ProductAttribute.AttributeName.ToLower().Contains(lowerKeyword))
                    )
                )
            );
        }

        query = ApplyFiltersAndSorting(query, startingPrice, toPrice, sortBy);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<ProductVariant>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<List<ProductVariant>> SearchVariantsAsync(string? keyword, decimal? startingPrice = null, decimal? toPrice = null, string? sortBy = null, int? pageNumber = null, int? pageSize = null)
    {
        var query = _context.ProductVariants
            .Include(pv => pv.ProductVariantImages)
            .Include(pv => pv.ProductVariantAttributes!)
                .ThenInclude(pva => pva.AttributeValue)
                    .ThenInclude(av => av!.ProductAttribute)
            .Include(pv => pv.Product)
                .ThenInclude(p => p!.Category)
            .Include(pv => pv.Product)
                .ThenInclude(p => p!.Brand)
            .Where(pv => pv.Product != null && !pv.Product.IsDeleted && pv.Status > 0);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var lowerKeyword = keyword.ToLower();
            query = query.Where(pv =>
                (pv.VariantName != null && pv.VariantName.ToLower().Contains(lowerKeyword))
                || pv.SKU.ToLower().Contains(lowerKeyword)
                || pv.Product!.ProductName.ToLower().Contains(lowerKeyword)
                || (pv.Product.Category != null && pv.Product.Category.Name.ToLower().Contains(lowerKeyword))
                || (pv.Product.Brand != null && pv.Product.Brand.Name.ToLower().Contains(lowerKeyword))
                || pv.ProductVariantAttributes!.Any(pva =>
                    pva.AttributeValue != null && (
                        pva.AttributeValue.Value.ToLower().Contains(lowerKeyword)
                        || (pva.AttributeValue.ProductAttribute != null && pva.AttributeValue.ProductAttribute.AttributeName.ToLower().Contains(lowerKeyword))
                    )
                )
            );
        }

        query = ApplyFiltersAndSorting(query, startingPrice, toPrice, sortBy);

        if (pageNumber.HasValue && pageSize.HasValue)
        {
            query = query.Skip((pageNumber.Value - 1) * pageSize.Value).Take(pageSize.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<List<ProductVariant>> GetNewestInStockAsync(int limit = 4)
    {
        if (limit <= 0)
        {
            return new List<ProductVariant>();
        }

        var productIds = await ActiveInStockVariants()
            .Select(pv => new
            {
                pv.ProductId,
                pv.Product!.CreatedAt
            })
            .Distinct()
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.ProductId)
            .Take(limit)
            .Select(x => x.ProductId)
            .ToListAsync();

        var variants = await ActiveInStockVariantsWithDetails()
            .Where(pv => productIds.Contains(pv.ProductId))
            .ToListAsync();

        return productIds
            .Select(productId => variants
                .Where(pv => pv.ProductId == productId)
                .OrderByDescending(HasMainImage)
                .ThenByDescending(pv => pv.StockQuantity)
                .ThenBy(pv => pv.Id)
                .FirstOrDefault())
            .Where(pv => pv is not null)
            .Select(pv => pv!)
            .ToList();
    }

    public async Task<List<ProductVariant>> GetBestSellingInStockAsync(int limit = 4)
    {
        if (limit <= 0)
        {
            return new List<ProductVariant>();
        }

        var salesByProduct = await _context.OrderItems
            .AsNoTracking()
            .Where(oi => !oi.Delete
                && oi.Order != null
                && !oi.Order.Delete
                && oi.Order.OrderStatus == "Hoàn thành"
                && oi.Order.PaymentStatus == "Đã thanh toán"
                && oi.ProductVariant != null
                && oi.ProductVariant.StockQuantity > 0
                && oi.ProductVariant.Status == 1
                && oi.ProductVariant.Product != null
                && !oi.ProductVariant.Product.IsDeleted
                && oi.ProductVariant.Product.Status == 1
                && oi.ProductVariant.Product.Category != null
                && !oi.ProductVariant.Product.Category.Delete
                && oi.ProductVariant.Product.Category.Status == 1
                && oi.ProductVariant.Product.Brand != null
                && !oi.ProductVariant.Product.Brand.Delete
                && oi.ProductVariant.Product.Brand.Status == 1)
            .GroupBy(oi => oi.ProductVariant!.ProductId)
            .Select(group => new
            {
                ProductId = group.Key,
                QuantitySold = group.Sum(oi => oi.Quantity)
            })
            .OrderByDescending(x => x.QuantitySold)
            .ThenByDescending(x => x.ProductId)
            .Take(limit)
            .ToListAsync();

        if (salesByProduct.Count == 0)
        {
            return new List<ProductVariant>();
        }

        var productIds = salesByProduct.Select(x => x.ProductId).ToList();
        var variants = await ActiveInStockVariantsWithDetails()
            .Where(pv => productIds.Contains(pv.ProductId))
            .ToListAsync();

        var salesByVariant = await _context.OrderItems
            .AsNoTracking()
            .Where(oi => !oi.Delete
                && oi.Order != null
                && !oi.Order.Delete
                && oi.Order.OrderStatus == "Hoàn thành"
                && oi.Order.PaymentStatus == "Đã thanh toán"
                && oi.ProductVariant != null
                && productIds.Contains(oi.ProductVariant.ProductId))
            .GroupBy(oi => oi.ProductVariantId)
            .Select(group => new
            {
                VariantId = group.Key,
                QuantitySold = group.Sum(oi => oi.Quantity)
            })
            .ToDictionaryAsync(x => x.VariantId, x => x.QuantitySold);

        return salesByProduct
            .Select(sale => variants
                .Where(pv => pv.ProductId == sale.ProductId)
                .OrderByDescending(pv => salesByVariant.GetValueOrDefault(pv.Id))
                .ThenByDescending(HasMainImage)
                .ThenByDescending(pv => pv.StockQuantity)
                .ThenBy(pv => pv.Id)
                .FirstOrDefault())
            .Where(pv => pv is not null)
            .Select(pv => pv!)
            .ToList();
    }

    public async Task<PagedResult<ProductVariant>> SearchForPosAsync(
        string? keyword,
        int? brandId,
        int? categoryId,
        int pageNumber = 1,
        int pageSize = 20)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _context.ProductVariants
            .Include(pv => pv.ProductVariantImages)
            .Include(pv => pv.ProductVariantAttributes!)
                .ThenInclude(pva => pva.AttributeValue)
                    .ThenInclude(av => av!.ProductAttribute)
            .Include(pv => pv.Product)
                .ThenInclude(p => p!.Category)
            .Include(pv => pv.Product)
                .ThenInclude(p => p!.Brand)
            .Where(pv =>    
                pv.Product != null
                && !pv.Product.IsDeleted
                && pv.Product.Status > 0
                && pv.Status > 0
                && pv.StockQuantity > 0);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var lowerKeyword = keyword.Trim().ToLower();
            query = query.Where(pv =>
                (pv.VariantName != null && pv.VariantName.ToLower().Contains(lowerKeyword))
                || pv.SKU.ToLower().Contains(lowerKeyword)
                || (pv.Product != null
                    && pv.Product.ProductName != null
                    && pv.Product.ProductName.ToLower().Contains(lowerKeyword)));
        }

        if (brandId.HasValue && brandId.Value > 0)
        {
            query = query.Where(pv => pv.Product!.BrandId == brandId.Value);
        }

        if (categoryId.HasValue && categoryId.Value > 0)
        {
            query = query.Where(pv => pv.Product!.CategoryId == categoryId.Value);
        }

        query = query
            .OrderBy(pv => pv.Product!.ProductName)
            .ThenBy(pv => pv.VariantName)
            .ThenBy(pv => pv.SKU);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<ProductVariant>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<List<ProductVariant>> GetVariantsByBrandIdAsync(int brandId, decimal? startingPrice = null, decimal? toPrice = null, string? sortBy = null)
    {
        var query = _context.ProductVariants
            .Include(pv => pv.ProductVariantImages)
            .Include(pv => pv.ProductVariantAttributes!)
                .ThenInclude(pva => pva.AttributeValue)
                    .ThenInclude(av => av!.ProductAttribute)
            .Include(pv => pv.Product)
                .ThenInclude(p => p!.Category)
            .Include(pv => pv.Product)
                .ThenInclude(p => p!.Brand)
            .Where(pv => pv.Product != null && !pv.Product.IsDeleted && pv.Status != -1 && pv.Product.BrandId == brandId);

        query = ApplyFiltersAndSorting(query, startingPrice, toPrice, sortBy);

        return await query.ToListAsync();
    }

    public async Task<List<ProductVariant>> GetVariantsByCategoryIdAsync(int categoryId, decimal? startingPrice = null, decimal? toPrice = null, string? sortBy = null)
    {
        var query = _context.ProductVariants
            .Include(pv => pv.ProductVariantImages)
            .Include(pv => pv.ProductVariantAttributes!)
                .ThenInclude(pva => pva.AttributeValue)
                    .ThenInclude(av => av!.ProductAttribute)
            .Include(pv => pv.Product)
                .ThenInclude(p => p!.Category)
            .Include(pv => pv.Product)
                .ThenInclude(p => p!.Brand)
            .Where(pv => pv.Product != null && !pv.Product.IsDeleted && pv.Status != -1 && pv.Product.CategoryId == categoryId);

        query = ApplyFiltersAndSorting(query, startingPrice, toPrice, sortBy);

        return await query.ToListAsync();
    }

    public async Task<List<ProductVariant>> GetVariantsByAttributeIdAsync(int attributeId, decimal? startingPrice = null, decimal? toPrice = null, string? sortBy = null)
    {
        var query = _context.ProductVariants
            .Include(pv => pv.ProductVariantImages)
            .Include(pv => pv.ProductVariantAttributes!)
                .ThenInclude(pva => pva.AttributeValue)
                    .ThenInclude(av => av!.ProductAttribute)
            .Include(pv => pv.Product)
                .ThenInclude(p => p!.Category)
            .Include(pv => pv.Product)
                .ThenInclude(p => p!.Brand)
            .Where(pv => pv.Product != null && !pv.Product.IsDeleted && pv.Status != -1 && pv.ProductVariantAttributes!.Any(pva =>
                pva.AttributeValue != null && pva.AttributeValue.AttributeId == attributeId));

        query = ApplyFiltersAndSorting(query, startingPrice, toPrice, sortBy);

        return await query.ToListAsync();
    }

    public async Task<bool> DecreaseStockAsync(int variantId, int quantity)
    {
        var variant = await _context.ProductVariants.FindAsync(variantId);
        if (variant == null)
            return false;

        int retries = 5;
        while (retries > 0)
        {
            try
            {
                if (variant.StockQuantity < quantity)
                    return false;

                variant.StockQuantity -= quantity;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                retries--;
                if (retries == 0)
                    return false;

                await _context.Entry(variant).ReloadAsync();
                await Task.Delay(50);
            }
        }
        return false;
    }

    public async Task<bool> IncreaseStockAsync(int variantId, int quantity)
    {
        var variant = await _context.ProductVariants.FindAsync(variantId);
        if (variant == null)
            return false;

        int retries = 5;
        while (retries > 0)
        {
            try
            {
                variant.StockQuantity += quantity;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                retries--;
                if (retries == 0)
                    return false;

                await _context.Entry(variant).ReloadAsync();
                await Task.Delay(50);
            }
        }
        return false;
    }

    public async Task<bool> HasActiveVariantsByProductIdAsync(int productId)
    {
        return await _context.ProductVariants.AnyAsync(pv => pv.ProductId == productId && pv.Status == 1);
    }

    private IQueryable<ProductVariant> ActiveInStockVariants()
    {
        return _context.ProductVariants
            .AsNoTracking()
            .Where(pv => pv.StockQuantity > 0
                && pv.Status == 1
                && pv.Product != null
                && !pv.Product.IsDeleted
                && pv.Product.Status == 1
                && pv.Product.Category != null
                && !pv.Product.Category.Delete
                && pv.Product.Category.Status == 1
                && pv.Product.Brand != null
                && !pv.Product.Brand.Delete
                && pv.Product.Brand.Status == 1
                && (pv.ProductVariantAttributes == null
                    || !pv.ProductVariantAttributes.Any()
                    || !pv.ProductVariantAttributes.Any(pva =>
                        pva.AttributeValue == null
                        || pva.AttributeValue.ProductAttribute == null)));
    }

    private IQueryable<ProductVariant> ActiveInStockVariantsWithDetails()
    {
        return ActiveInStockVariants()
            .Include(pv => pv.ProductVariantImages)
            .Include(pv => pv.ProductVariantAttributes!)
                .ThenInclude(pva => pva.AttributeValue)
                    .ThenInclude(av => av!.ProductAttribute)
            .Include(pv => pv.Product)
                .ThenInclude(p => p!.Category)
            .Include(pv => pv.Product)
                .ThenInclude(p => p!.Brand)
            .AsSplitQuery();
    }

    private static bool HasMainImage(ProductVariant variant)
    {
        return variant.ProductVariantImages?.Any(image => image.IsMain) == true;
    }
}
