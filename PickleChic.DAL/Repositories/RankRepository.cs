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
        return await _context.Ranks
            .Where(r => !r.Delete)
            .ToListAsync();
    }

    public async Task<Rank?> GetByIdAsync(int id)
    {
        return await _context.Ranks.FindAsync(id);
    }

    public async Task<Rank> AddAsync(Rank entity)
    {
        entity.Delete = false;
        _context.Ranks.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Rank?> UpdateAsync(Rank entity)
    {
        try
        {
            var existing = await _context.Ranks.FindAsync(entity.Id);
            if (existing is null || existing.Delete)
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

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var entity = await _context.Ranks.FirstOrDefaultAsync(r => r.Id == id);
        if (entity is null || entity.Delete)
            return false;

        entity.Delete = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByNameAsync(string rankName, int? excludeId = null)
    {
        var normalized = rankName.Trim().ToLower();
        return await _context.Ranks.AnyAsync(r =>
            !r.Delete
            && r.RankName.ToLower() == normalized
            && (!excludeId.HasValue || r.Id != excludeId.Value));
    }

    public async Task<bool> ExistsBySpendAmountAsync(decimal spendAmount, int? excludeId = null)
    {
        return await _context.Ranks.AnyAsync(r =>
            !r.Delete
            && r.SpendAmount == spendAmount
            && (!excludeId.HasValue || r.Id != excludeId.Value));
    }
}
