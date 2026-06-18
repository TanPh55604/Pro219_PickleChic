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

    public async Task<List<ProductAttribute>> GetAllByCategoryId(int categoryId)
    {
        return await _context.ProductAttributes
            .Include(a => a.AttributeValues).Where(x=>x.CategoryId== categoryId)
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

    public async Task<ProductAttribute?> UpdateWithValuesAndFlagAsync(
        int id,
        string attributeName,
        List<(int Id, string Value, string? Note, int FlagAction)> values)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var existing = await _context.ProductAttributes
                .Include(a => a.AttributeValues)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (existing is null)
                return null;

            existing.AttributeName = attributeName;

            foreach (var item in values)
            {
                if (item.FlagAction == 1) 
                {
                    var newValue = new AttributeValue
                    {
                        AttributeId = id,
                        Value = item.Value,
                        Note = item.Note
                    };
                    _context.AttributeValues.Add(newValue);
                }
                else if (item.FlagAction == 2) 
                {
                    var existingVal = existing.AttributeValues?.FirstOrDefault(v => v.Id == item.Id);
                    if (existingVal != null)
                    {
                        existingVal.Value = item.Value;
                        existingVal.Note = item.Note;
                    }
                }
                else if (item.FlagAction == 3) 
                {
                    var existingVal = existing.AttributeValues?.FirstOrDefault(v => v.Id == item.Id);
                    if (existingVal != null)
                    {
                        _context.AttributeValues.Remove(existingVal);
                    }
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return await GetByIdAsync(id);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
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
