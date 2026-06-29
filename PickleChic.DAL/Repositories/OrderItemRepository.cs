using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class OrderItemRepository
{
    private readonly PickleChicDbContext _context;

    public OrderItemRepository(PickleChicDbContext context)
    {
        _context = context;
    }

    public async Task<List<OrderItem>> GetAllAsync()
    {
        return await _context.OrderItems
            .Where(oi => !oi.Delete
                && _context.Orders.Any(o => o.Id == oi.OrderId && !o.Delete))
            .ToListAsync();
    }

    public async Task<OrderItem?> GetByIdAsync(int id)
    {
        return await _context.OrderItems
            .FirstOrDefaultAsync(oi => oi.Id == id && !oi.Delete
                && _context.Orders.Any(o => o.Id == oi.OrderId && !o.Delete));
    }

    public async Task<OrderItem> AddAsync(OrderItem entity)
    {
        _context.OrderItems.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<OrderItem?> UpdateAsync(OrderItem entity)
    {
        try
        {
            var existing = await _context.OrderItems.FindAsync(entity.Id);
            if (existing is null)
                return null;
            if (existing.Delete)
                return null;

            existing.OrderId = entity.OrderId;
            existing.ProductVariantId = entity.ProductVariantId;
            existing.PromotionId = entity.PromotionId;
            existing.Quantity = entity.Quantity;
            existing.UnitPrice = entity.UnitPrice;
            existing.DiscountAmount = entity.DiscountAmount;
            existing.Subtotal = entity.Subtotal;
            existing.IsReviewed = entity.IsReviewed;
            existing.UpdateBy = entity.UpdateBy;
            existing.UpdateAt = entity.UpdateAt ?? DateTime.Now;
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
        var entity = await _context.OrderItems.FirstOrDefaultAsync(oi => oi.Id == id);
        if (entity is null || entity.Delete)
            return false;

        entity.Delete = true;
        entity.DeleteAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }
}
