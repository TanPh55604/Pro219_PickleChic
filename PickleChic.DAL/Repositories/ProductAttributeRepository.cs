using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class ProductAttributeRepository
{
    private readonly PickleChicDbContext _context;

    public ProductAttributeRepository(PickleChicDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductAttribute>> GetAllAsync()
    {
        return await _context.ProductAttributes
            .Include(a => a.AttributeValues)
            .ToListAsync();
    }

    public async Task<ProductAttribute?> GetByIdAsync(int id)
    {
        return await _context.ProductAttributes
            .Include(a => a.AttributeValues)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<ProductAttribute> AddAsync(ProductAttribute entity)
    {
        _context.ProductAttributes.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<ProductAttribute?> UpdateAsync(ProductAttribute entity)
    {
        try
        {
            var existing = await _context.ProductAttributes.FindAsync(entity.Id);
            if (existing is null)
                return null;

            existing.AttributeName = entity.AttributeName;
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
        var entity = await _context.ProductAttributes.FindAsync(id);
        if (entity is null)
            return false;

        _context.ProductAttributes.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
