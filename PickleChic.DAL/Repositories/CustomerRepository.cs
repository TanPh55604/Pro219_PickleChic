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
        return await _context.Customers.ToListAsync();
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
        return await _context.Customers.FindAsync(id);
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
}
