using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class AttributeValueRepository
{
    private readonly PickleChicDbContext _context;

    public AttributeValueRepository(PickleChicDbContext context)
    {
        _context = context;
    }

    public async Task<List<AttributeValue>> GetAllAsync()
    {
        return await _context.AttributeValues
            .Include(av => av.ProductAttribute)
            .ToListAsync();
    }

    public async Task<AttributeValue?> GetByIdAsync(int id)
    {
        return await _context.AttributeValues
            .Include(av => av.ProductAttribute)
            .FirstOrDefaultAsync(av => av.Id == id);
    }

    public async Task<AttributeValue> AddAsync(AttributeValue entity)
    {
        _context.AttributeValues.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<AttributeValue?> UpdateAsync(AttributeValue entity)
    {
        try
        {
            var existing = await _context.AttributeValues.FindAsync(entity.Id);
            if (existing is null)
                return null;

            existing.AttributeId = entity.AttributeId;
            existing.Value = entity.Value;
            existing.Note = entity.Note;
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
        var entity = await _context.AttributeValues.FindAsync(id);
        if (entity is null)
            return false;

        _context.AttributeValues.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
