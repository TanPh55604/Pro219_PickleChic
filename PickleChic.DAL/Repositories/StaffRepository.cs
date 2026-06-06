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
        return await _context.Staff.FirstOrDefaultAsync(s => (s.UserName == userName || s.Email == userName) && s.PasswordHash == passwordHash);
    }

    public async Task<List<Staff>> GetAllAsync()
    {
        return await _context.Staff.Where(x => x.Status != -1).ToListAsync();
    }

    public async Task<Staff?> GetByIdAsync(int id)
    {
        var staff = await _context.Staff.FindAsync(id);
        if (staff != null && staff.Status != -1)
            return staff;
        return null;
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

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var entity = await _context.Staff.FindAsync(id);
        if (entity is null)
            return false;

        entity.Status = -1;
        _context.Staff.Update(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
