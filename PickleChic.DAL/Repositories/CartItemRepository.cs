using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class CartItemRepository
{
    private readonly PickleChicDbContext _context;

    public CartItemRepository(PickleChicDbContext context)
    {
        _context = context;
    }

    public async Task<List<CartItem>> GetAllAsync()
    {
        return await _context.CartItems
            .Include(ci => ci.ProductVariant)
                .ThenInclude(pv => pv!.Product)
            .Where(ci => _context.ProductVariants.Any(pv =>
                pv.Id == ci.ProductVariantId
                && _context.Products.Any(p => p.Id == pv.ProductId && !p.IsDeleted)))
            .ToListAsync();
    }

    public async Task<CartItem?> GetByIdAsync(int id)
    {
        return await _context.CartItems
            .Include(ci => ci.ProductVariant)
                .ThenInclude(pv => pv!.Product)
            .FirstOrDefaultAsync(ci => ci.Id == id
                && _context.ProductVariants.Any(pv =>
                    pv.Id == ci.ProductVariantId
                    && _context.Products.Any(p => p.Id == pv.ProductId && !p.IsDeleted)));
    }

    public async Task<List<CartItem>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.CartItems
            .Include(ci => ci.ProductVariant)
                .ThenInclude(pv => pv!.Product)
            .Where(ci => ci.CustomerId == customerId
                && _context.ProductVariants.Any(pv =>
                    pv.Id == ci.ProductVariantId
                    && _context.Products.Any(p => p.Id == pv.ProductId && !p.IsDeleted)))
            .ToListAsync();
    }

    public async Task<CartItem> AddAsync(CartItem entity)
    {
        _context.CartItems.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<CartItem?> UpdateAsync(CartItem entity)
    {
        try
        {
            var existing = await _context.CartItems.FindAsync(entity.Id);
            if (existing is null)
                return null;

            existing.CustomerId = entity.CustomerId;
            existing.ProductVariantId = entity.ProductVariantId;
            existing.Quantity = entity.Quantity;
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
        var entity = await _context.CartItems.FindAsync(id);
        if (entity is null)
            return false;

        _context.CartItems.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
