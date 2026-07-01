using Hangfire;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace PickleChic.API.Services;

public class OrderManagerService
{
    private readonly OrderRepository _orderRepository;
    private readonly ProductVariantRepository _productVariantRepository;
    private readonly VoucherRepository _voucherRepository;

    public OrderManagerService(
        OrderRepository orderRepository, 
        ProductVariantRepository productVariantRepository,
        VoucherRepository voucherRepository)
    {
        _orderRepository = orderRepository;
        _productVariantRepository = productVariantRepository;
        _voucherRepository = voucherRepository;
    }

    public async Task CancelExpiredOrderAsync(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order != null && order.PaymentStatus == Constant.PaymentStatus.Pending && order.PaymentExpiration < DateTime.Now)
        {
            order.OrderStatus = Constant.OrderStatus.Expired;
            order.PaymentStatus = Constant.PaymentStatus.Cancelled;
            order.LastUpdate = DateTime.Now;
            order.UpdateBy = "System";

            await _orderRepository.UpdateAsync(order);

            if (order.OrderItems != null && order.OrderItems.Any())
            {
                foreach (var orderItem in order.OrderItems)
                {
                    await _productVariantRepository.IncreaseStockAsync(orderItem.ProductVariantId, orderItem.Quantity);
                }
            }

            if (order.VoucherId != null)
            {
                var voucher = await _voucherRepository.GetByIdAsync(order.VoucherId.Value);
                if (voucher != null && voucher.UsedCount > 0)
                {
                    voucher.UsedCount--;
                    await _voucherRepository.UpdateAsync(voucher);
                }
            }
        }
    }
}
