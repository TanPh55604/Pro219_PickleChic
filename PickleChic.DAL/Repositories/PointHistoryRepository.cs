using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class PointHistoryRepository
{
    private readonly PickleChicDbContext _context;

    public PointHistoryRepository()
    {
        _context = new PickleChicDbContext();
    }

    public PointHistoryRepository(PickleChicDbContext context)
    {
        _context = context;
    }

    public async Task<List<PointHistory>> GetAllAsync()
    {
        return await _context.PointHistories
            .Where(ph => _context.Orders.Any(o => o.Id == ph.OrderId && !o.Delete))
            .ToListAsync();
    }

    public async Task<PointHistory?> GetByIdAsync(int id)
    {
        return await _context.PointHistories
            .FirstOrDefaultAsync(ph => ph.Id == id
                && _context.Orders.Any(o => o.Id == ph.OrderId && !o.Delete));
    }

    public async Task<PointHistory> AddAsync(PointHistory entity)
    {
        _context.PointHistories.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<PointHistory?> UpdateAsync(PointHistory entity)
    {
        try
        {
            var existing = await _context.PointHistories.FindAsync(entity.Id);
            if (existing is null)
                return null;

            existing.CustomerId = entity.CustomerId;
            existing.OrderId = entity.OrderId;
            existing.Points = entity.Points;
            existing.TransactionType = entity.TransactionType;
            existing.Description = entity.Description;
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
        var entity = await _context.PointHistories.FindAsync(id);
        if (entity is null)
            return false;

        _context.PointHistories.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int> GetAccumulatedPointsInLast6MonthsAsync(int customerId)
    {
        var sixMonthsAgo = DateTime.Now.AddMonths(-6);
        return await _context.PointHistories
            .Where(ph => ph.CustomerId == customerId && ph.CreatedAt >= sixMonthsAgo && ph.Points > 0)
            .SumAsync(ph => ph.Points);
    }

    public async Task<bool> RefundPointsForOrderAsync(int orderId)
    {
        try
        {
            // 1. Check if we already refunded points for this order to avoid double refunding
            bool alreadyRefunded = await _context.PointHistories
                .AnyAsync(ph => ph.OrderId == orderId && ph.TransactionType == "Hoàn điểm");
            if (alreadyRefunded)
            {
                return false;
            }

            // 2. Find the points used (deducted) for this order
            var deduction = await _context.PointHistories
                .FirstOrDefaultAsync(ph => ph.OrderId == orderId && ph.Points < 0 && ph.TransactionType == "Dùng điểm");

            if (deduction != null)
            {
                int pointsToRefund = Math.Abs(deduction.Points);

                // 3. Load the customer
                var customer = await _context.Customers.FindAsync(deduction.CustomerId);
                if (customer != null)
                {
                    customer.TotalPoints += pointsToRefund;

                    // 4. Load the order to get the order code
                    var order = await _context.Orders.FindAsync(orderId);
                    string orderCode = order?.OrderCode ?? orderId.ToString();

                    // 5. Add a positive PointHistory entry
                    var refundHistory = new PointHistory
                    {
                        CustomerId = customer.Id,
                        OrderId = orderId,
                        Points = pointsToRefund,
                        TransactionType = "Hoàn điểm",
                        Description = $"Hoàn điểm từ đơn hàng bị hủy {orderCode}",
                        CreatedAt = DateTime.Now
                    };

                    _context.PointHistories.Add(refundHistory);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<List<PointHistory>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.PointHistories
            .Where(ph => ph.CustomerId == customerId)
            .OrderByDescending(ph => ph.CreatedAt)
            .ToListAsync();
    }
}
