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
            .Include(r => r.OrderItem)
                .ThenInclude(oi => oi.Order)
                    .ThenInclude(o => o.Customer)
            .Where(r => !r.Delete)
            .ToListAsync();
    }

    public async Task<Review?> GetByIdAsync(int id)
    {
        return await _context.Reviews
            .Include(r => r.ProductVariant)
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
        return await _context.Reviews
            .AnyAsync(r => r.ProductVariantId == productVariantId 
                && r.OrderItem != null 
                && r.OrderItem.Order != null 
                && r.OrderItem.Order.CustomerId == customerId 
                && !r.Delete);
    }

    public async Task<OrderItem?> GetEligibleOrderItemAsync(int customerId, int productVariantId)
    {
        return await _context.OrderItems
            .Include(oi => oi.Order)
            .FirstOrDefaultAsync(oi => oi.Order != null
                && oi.Order.CustomerId == customerId
                && oi.ProductVariantId == productVariantId
                && oi.Order.OrderStatus == "Hoàn thành"
                && !oi.Order.Delete
                && !oi.Delete);
    }

    public async Task<List<OrderItem>> GetUnreviewedItemsByCustomerIdAsync(int customerId)
    {
        var reviewedVariantIds = await _context.Reviews
            .Where(r => r.OrderItem != null && r.OrderItem.Order != null && r.OrderItem.Order.CustomerId == customerId && !r.Delete)
            .Select(r => r.ProductVariantId)
            .Distinct()
            .ToListAsync();

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
