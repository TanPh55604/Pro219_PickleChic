using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class WishlistRepository
{
    private readonly PickleChicDbContext _context;

    public WishlistRepository(PickleChicDbContext context)
    {
        _context = context;
    }

    public async Task<List<Wishlist>> GetAllAsync()
    {
        return await _context.Wishlists
            .Where(w => _context.Products.Any(p => p.Id == w.ProductId && !p.IsDeleted))
            .ToListAsync();
    }

    public async Task<List<Wishlist>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.Wishlists
            .Where(w => w.CustomerId == customerId
                && _context.Products.Any(p => p.Id == w.ProductId && !p.IsDeleted))
            .ToListAsync();
    }

    public async Task<Wishlist?> GetByIdAsync(int id)
    {
        return await _context.Wishlists
            .FirstOrDefaultAsync(w => w.Id == id
                && _context.Products.Any(p => p.Id == w.ProductId && !p.IsDeleted));
    }

    public async Task<Wishlist> AddAsync(Wishlist entity)
    {
        _context.Wishlists.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Wishlist?> UpdateAsync(Wishlist entity)
    {
        try
        {
            var existing = await _context.Wishlists.FindAsync(entity.Id);
            if (existing is null)
                return null;

            existing.CustomerId = entity.CustomerId;
            existing.ProductId = entity.ProductId;
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
        var entity = await _context.Wishlists.FindAsync(id);
        if (entity is null)
            return false;

        _context.Wishlists.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
