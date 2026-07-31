using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class CustomerRepository
{
    private readonly PickleChicDbContext _context;

    public CustomerRepository()
    {
        _context = new PickleChicDbContext();
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        return await _context.Customers.Where(x=>x.IsDeleted!=true).ToListAsync();
    }

    public async Task<Customer> FindUserExistByKeyWord(string key)
    {
        try
        {
            var c = _context.Customers.FirstOrDefault(x => (x.Email == key || x.PhoneNumber == key || x.Username == key));

            if (c == null)
                return null;
            return c;


        }
        catch
        {
            return null;
        }
    }

    public async Task<Customer> FindUserByEmailAndPhoneAndUserName(string email, string phoneNumber, string username)
    {
        try
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => (x.Email == email || x.PhoneNumber == phoneNumber || x.Email == username));
            return customer;
        }
        catch
        {
            return null;
        }
    }

    public async Task<Customer> GetByKeyAndPassword(string userName, string passwordHash)
    {
        return await _context.Customers.FirstOrDefaultAsync(c => c.Email == userName && c.PasswordHash == passwordHash || c.Username == userName && c.PasswordHash == passwordHash);
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        Customer cus =  await _context.Customers.FindAsync(id);
        if (cus != null && cus.IsDeleted != true)
            return cus;
        return null;
    }

    public async Task<(List<Customer> Items, int TotalCount)> SearchForPosAsync(
        string? keyword,
        int pageNumber = 1,
        int pageSize = 20)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var query = _context.Customers
            .Include(c => c.Rank)
            .Where(c => c.IsDeleted != true && c.Status > 0 && c.Id > 0);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var lowerKeyword = keyword.Trim().ToLower();
            query = query.Where(c =>
                c.FullName.ToLower().Contains(lowerKeyword)
                || c.Email.ToLower().Contains(lowerKeyword)
                || (c.PhoneNumber != null && c.PhoneNumber.ToLower().Contains(lowerKeyword)));
        }

        query = query.OrderBy(c => c.FullName).ThenBy(c => c.Id);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<Customer> AddAsync(Customer entity)
    {
        _context.Customers.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Customer?> UpdateAsync(Customer entity)
    {
        try
        {
            var existing = await _context.Customers.FindAsync(entity.Id);
            if (existing is null)
                return null;
            existing.Username = entity.Username;
            existing.FullName = entity.FullName;
            existing.Email = entity.Email;
            existing.PasswordHash = entity.PasswordHash;
            existing.PhoneNumber = entity.PhoneNumber;
            existing.Gender = entity.Gender;
            existing.DateOfBirth = entity.DateOfBirth;
            existing.TotalPoints = entity.TotalPoints;
            existing.LastLogin = entity.LastLogin;
            existing.Status = entity.Status;
            existing.RankId = entity.RankId;
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
        var entity = await _context.Customers.FindAsync(id);
        if (entity is null)
            return false;

        _context.Customers.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var entity = await _context.Customers.FindAsync(id);
        if (entity is null)
            return false;

        entity.IsDeleted = true;
        _context.Customers.Update(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
