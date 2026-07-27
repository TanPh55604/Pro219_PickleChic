using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class ReviewRepository
{
    private readonly PickleChicDbContext _context;

    public ReviewRepository(PickleChicDbContext context)
    {
        _context = context;
    }

    public async Task<List<Review>> GetAllAsync()
    {
        return await _context.Reviews
            .Include(r => r.ProductVariant)
                .ThenInclude(pv => pv.Product)
            .Include(r => r.OrderItem)
                .ThenInclude(oi => oi.Order)
                    .ThenInclude(o => o.Customer)
            .Where(r => !r.Delete)
            .OrderByDescending(r => r.CreateAt)
            .ToListAsync();
    }

    public async Task<List<Review>> SearchAsync(string? keyword = null, int? status = null)
    {
        var query = _context.Reviews
            .Include(r => r.ProductVariant)
                .ThenInclude(pv => pv.Product)
            .Include(r => r.OrderItem)
                .ThenInclude(oi => oi.Order)
                    .ThenInclude(o => o.Customer)
            .Where(r => !r.Delete);

        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var key = keyword.Trim().ToLower();
            query = query.Where(r =>
                (r.Title != null && r.Title.ToLower().Contains(key))
                || r.Content.ToLower().Contains(key)
                || (r.ProductVariant != null && r.ProductVariant.SKU != null && r.ProductVariant.SKU.ToLower().Contains(key))
                || (r.ProductVariant != null && r.ProductVariant.VariantName != null && r.ProductVariant.VariantName.ToLower().Contains(key))
                || (r.ProductVariant != null && r.ProductVariant.Product != null && r.ProductVariant.Product.ProductName.ToLower().Contains(key))
                || (r.OrderItem != null && r.OrderItem.Order != null && r.OrderItem.Order.Customer != null
                    && r.OrderItem.Order.Customer.FullName != null
                    && r.OrderItem.Order.Customer.FullName.ToLower().Contains(key))
                || (r.OrderItem != null && r.OrderItem.Order != null && r.OrderItem.Order.Customer != null
                    && r.OrderItem.Order.Customer.Username != null
                    && r.OrderItem.Order.Customer.Username.ToLower().Contains(key)));
        }

        return await query
            .OrderByDescending(r => r.CreateAt)
            .ToListAsync();
    }

    public async Task<Review?> GetByIdAsync(int id)
    {
        return await _context.Reviews
            .Include(r => r.ProductVariant)
                .ThenInclude(pv => pv.Product)
            .Include(r => r.OrderItem)
                .ThenInclude(oi => oi.Order)
                    .ThenInclude(o => o.Customer)
            .FirstOrDefaultAsync(r => r.Id == id && !r.Delete);
    }

    public async Task<Review> AddAsync(Review entity)
    {
        _context.Reviews.Add(entity);

        var orderItem = await _context.OrderItems.FindAsync(entity.OrderItemId);
        if (orderItem != null)
        {
            orderItem.IsReviewed = true;
        }

        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Review?> UpdateAsync(Review entity)
    {
        try
        {
            var existing = await _context.Reviews.FindAsync(entity.Id);
            if (existing is null || existing.Delete)
                return null;

            existing.Title = entity.Title;
            existing.Content = entity.Content;
            existing.Overall = entity.Overall;
            existing.Status = entity.Status;

            await _context.SaveChangesAsync();
            return existing;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<Review?> UpdateStatusAsync(int id, int status)
    {
        var existing = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id && !r.Delete);
        if (existing is null)
            return null;

        existing.Status = status;
        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var entity = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
        if (entity is null || entity.Delete)
            return false;

        entity.Delete = true;
        var orderItem = await _context.OrderItems.FindAsync(entity.OrderItemId);
        if (orderItem != null)
        {
            orderItem.IsReviewed = false;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == id);
        if (entity is null)
            return false;

        _context.Reviews.Remove(entity);
        var orderItem = await _context.OrderItems.FindAsync(entity.OrderItemId);
        if (orderItem != null)
        {
            orderItem.IsReviewed = false;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<Review>> GetReviewsByProductVariantIdAsync(int productVariantId)
    {
        return await _context.Reviews
            .Include(r => r.OrderItem)
                .ThenInclude(oi => oi.Order)
                    .ThenInclude(o => o.Customer)
            .Where(r => r.ProductVariantId == productVariantId && !r.Delete && r.Status == 1)
            .OrderByDescending(r => r.CreateAt)
            .ToListAsync();
    }

    public async Task<bool> HasCustomerReviewedVariantAsync(int customerId, int productVariantId)
    {
        return await (
            from r in _context.Reviews
            join oi in _context.OrderItems on r.OrderItemId equals oi.Id
            join o in _context.Orders on oi.OrderId equals o.Id
            where r.ProductVariantId == productVariantId && o.CustomerId == customerId
            select r.Id
        ).AnyAsync();
    }

    public async Task<Review?> GetCustomerReviewForVariantAsync(int customerId, int productVariantId)
    {
        return await (
            from r in _context.Reviews
            join oi in _context.OrderItems on r.OrderItemId equals oi.Id
            join o in _context.Orders on oi.OrderId equals o.Id
            where r.ProductVariantId == productVariantId && o.CustomerId == customerId
            orderby r.CreateAt descending
            select r
        ).FirstOrDefaultAsync();
    }

    public async Task<OrderItem?> GetEligibleOrderItemAsync(int customerId, int productVariantId)
    {
        return await _context.OrderItems
            .Include(oi => oi.Order)
            .Where(oi => oi.Order != null
                && oi.Order.CustomerId == customerId
                && oi.ProductVariantId == productVariantId
                && oi.Order.OrderStatus == "Hoàn thành"
                && !oi.Order.Delete
                && !oi.Delete)
            .OrderByDescending(oi => oi.Id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<OrderItem>> GetUnreviewedItemsByCustomerIdAsync(int customerId)
    {
        var reviewedVariantIds = await (
            from r in _context.Reviews
            join oi in _context.OrderItems on r.OrderItemId equals oi.Id
            join o in _context.Orders on oi.OrderId equals o.Id
            where o.CustomerId == customerId
            select r.ProductVariantId
        ).Distinct().ToListAsync();

        var orderItems = await _context.OrderItems
            .Include(oi => oi.ProductVariant)
                .ThenInclude(pv => pv.Product)
            .Include(oi => oi.ProductVariant)
                .ThenInclude(pv => pv.ProductVariantImages)
            .Include(oi => oi.Order)
            .Where(oi => oi.Order != null
                && oi.Order.CustomerId == customerId
                && oi.Order.OrderStatus == "Hoàn thành"
                && !oi.Order.Delete
                && !oi.Delete
                && !reviewedVariantIds.Contains(oi.ProductVariantId))
            .ToListAsync();

        var uniqueUnreviewedItems = orderItems
            .GroupBy(oi => oi.ProductVariantId)
            .Select(g => g.OrderByDescending(oi => oi.Id).First())
            .ToList();

        return uniqueUnreviewedItems;
    }
}
