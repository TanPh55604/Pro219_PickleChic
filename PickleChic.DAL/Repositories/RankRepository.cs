using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class RankRepository
{
    private readonly PickleChicDbContext _context;

    public RankRepository()
    {
        _context = new PickleChicDbContext();
    }

    public async Task<List<Rank>> GetAllAsync()
    {
        return await _context.Ranks.ToListAsync();
    }

    public async Task<Rank?> GetByIdAsync(int id)
    {
        return await _context.Ranks.FindAsync(id);
    }

    public async Task<Rank> AddAsync(Rank entity)
    {
        _context.Ranks.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Rank?> UpdateAsync(Rank entity)
    {
        try
        {
            var existing = await _context.Ranks.FindAsync(entity.Id);
            if (existing is null)
                return null;

            existing.RankName = entity.RankName;
            existing.SpendAmount = entity.SpendAmount;
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
        var entity = await _context.Ranks.FindAsync(id);
        if (entity is null)
            return false;

        _context.Ranks.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByNameAsync(string rankName, int? excludeId = null)
    {
        var normalized = rankName.Trim().ToLower();
        return await _context.Ranks.AnyAsync(r =>
            r.RankName.ToLower() == normalized
            && (!excludeId.HasValue || r.Id != excludeId.Value));
    }
}
