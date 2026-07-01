using Microsoft.EntityFrameworkCore;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.DAL.Repositories;

public class OrderRepository
{
    private readonly PickleChicDbContext _context;

    public OrderRepository(PickleChicDbContext context)
    {
        _context = context;
    }

    public async Task<List<Order>> GetAllAsync()
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Address)
                .ThenInclude(a => a!.Ward)
                    .ThenInclude(w => w!.District)
                        .ThenInclude(d => d!.Province)
            .Include(o => o.PaymentMethod)
            .Include(o => o.Voucher)
            .Where(o => !o.Delete)
            .ToListAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Address)
                .ThenInclude(a => a!.Ward)
                    .ThenInclude(w => w!.District)
                        .ThenInclude(d => d!.Province)
            .Include(o => o.PaymentMethod)
            .Include(o => o.Voucher)
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == id && !o.Delete);
    }

    public async Task<Order> AddAsync(Order entity)
    {
        _context.Orders.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<Order?> UpdateAsync(Order entity)
    {
        try
        {
            var existing = await _context.Orders.FindAsync(entity.Id);
            if (existing is null)
                return null;
            if (existing.Delete)
                return null;

            existing.CustomerId = entity.CustomerId;
            existing.OrderCode = entity.OrderCode;
            existing.OrderDate = entity.OrderDate;
            existing.AddressId = entity.AddressId;
            existing.ShippingFee = entity.ShippingFee;
            existing.PaymentMethodId = entity.PaymentMethodId;
            existing.VoucherId = entity.VoucherId;
            existing.PaymentStatus = entity.PaymentStatus;
            existing.OrderStatus = entity.OrderStatus;
            existing.Notes = entity.Notes;
            existing.StatusHistory = entity.StatusHistory;
            existing.CustomerType = entity.CustomerType;
            existing.IsOrderPOS = entity.IsOrderPOS;
            existing.PaymentLink = entity.PaymentLink;
            existing.PaymentExpiration = entity.PaymentExpiration;
            existing.UpdateBy = entity.UpdateBy;
            existing.LastUpdate = entity.LastUpdate ?? DateTime.Now;
            await _context.SaveChangesAsync();
            return existing;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<int> GetVoucherUsageCountAsync(int customerId, int voucherId, string? phoneNumber = null)
    {
        if (customerId != -1)
        {
            return await _context.Orders
                .CountAsync(o => o.CustomerId == customerId 
                                 && o.VoucherId == voucherId 
                                 && !o.Delete 
                                 && o.OrderStatus != "Cancelled"
                                 && o.OrderStatus != "Đã hủy"
                                 && o.OrderStatus != "Expired"
                                 && o.OrderStatus != "Đã hết hạn");
        }
        else if (!string.IsNullOrEmpty(phoneNumber))
        {
            return await _context.Orders
                .Include(o => o.Address)
                .CountAsync(o => o.VoucherId == voucherId 
                                 && o.Address != null 
                                 && o.Address.PhoneNumber == phoneNumber 
                                 && !o.Delete 
                                 && o.OrderStatus != "Cancelled"
                                 && o.OrderStatus != "Đã hủy"
                                 && o.OrderStatus != "Expired"
                                 && o.OrderStatus != "Đã hết hạn");
        }
        return 0;
    }

    public async Task<Order?> GetOrderDetailForUserAsync(int orderId, int customerId)
    {
        return await _context.Orders
            .Include(o => o.Address)
                .ThenInclude(a => a!.Ward)
                    .ThenInclude(w => w!.District)
                        .ThenInclude(d => d!.Province)
            .Include(o => o.Voucher)
            .Include(o => o.OrderItems!)
                .ThenInclude(oi => oi.ProductVariant!)
                    .ThenInclude(pv => pv.Product)
            .Include(o => o.OrderItems!)
                .ThenInclude(oi => oi.ProductVariant!)
                    .ThenInclude(pv => pv.ProductVariantAttributes!)
                        .ThenInclude(pva => pva.AttributeValue!)
                            .ThenInclude(av => av.ProductAttribute)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.CustomerId == customerId && !o.Delete);
    }

    public async Task<List<Order>> GetOrdersForUserAsync(int customerId)
    {
        return await _context.Orders
            .Include(o => o.Address)
                .ThenInclude(a => a!.Ward)
                    .ThenInclude(w => w!.District)
                        .ThenInclude(d => d!.Province)
            .Include(o => o.Voucher)
            .Include(o => o.OrderItems!)
                .ThenInclude(oi => oi.ProductVariant!)
                    .ThenInclude(pv => pv.Product)
            .Include(o => o.OrderItems!)
                .ThenInclude(oi => oi.ProductVariant!)
                    .ThenInclude(pv => pv.ProductVariantAttributes!)
                        .ThenInclude(pva => pva.AttributeValue!)
                            .ThenInclude(av => av.ProductAttribute)
            .Where(o => o.CustomerId == customerId && !o.Delete)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id);
        if (entity is null || entity.Delete)
            return false;

        entity.Delete = true;
        entity.DeleteAt = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }
}
