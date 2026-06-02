using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class StaffRepository
{
    private readonly PickleChicDbContext _context;

    public StaffRepository()
    {
        _context = new PickleChicDbContext();
    }

    public async Task<Staff> GetByKeyAndPassword(string userName, string passwordHash)
    {
        return await _context.Staff.FirstOrDefaultAsync(s => s.UserName == userName && s.PasswordHash == passwordHash);
    }

    public async Task<List<Staff>> GetAllAsync()
    {
        return await _context.Staff.ToListAsync();
    }

    public async Task<Staff?> GetByIdAsync(int id)
    {
        return await _context.Staff.FindAsync(id);
    }

    public async Task<Staff> AddAsync(Staff entity)
    {
        _context.Staff.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Staff?> UpdateAsync(Staff entity)
    {
        try
        {
            var existing = await _context.Staff.FindAsync(entity.Id);
            if (existing is null)
                return null;

            existing.FullName = entity.FullName;
            existing.UserName = entity.UserName;
            existing.Email = entity.Email;
            existing.PhoneNumber = entity.PhoneNumber;
            existing.PasswordHash = entity.PasswordHash;
            existing.RoleId = entity.RoleId;
            existing.LastLogin = entity.LastLogin;
            existing.Status = entity.Status;
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
        var entity = await _context.Staff.FindAsync(id);
        if (entity is null)
            return false;

        _context.Staff.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
