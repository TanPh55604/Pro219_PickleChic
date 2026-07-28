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
    private readonly PointHistoryRepository _pointHistoryRepository;

    public OrderManagerService(
        OrderRepository orderRepository, 
        ProductVariantRepository productVariantRepository,
        VoucherRepository voucherRepository,
        PointHistoryRepository pointHistoryRepository)
    {
        _orderRepository = orderRepository;
        _productVariantRepository = productVariantRepository;
        _voucherRepository = voucherRepository;
        _pointHistoryRepository = pointHistoryRepository;
    }

    public async Task CancelExpiredOrderAsync(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order != null && order.PaymentStatus == Constant.PaymentStatus.Pending && order.PaymentExpiration < DateTime.Now)
        {
            order.OrderStatus = Constant.OrderStatus.Expired;
            order.Status = Constant.OrderStatus.GetStatusInt(order.OrderStatus);
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

            await _pointHistoryRepository.RefundPointsForOrderAsync(orderId);
        }
    }

    public async Task ActivateVoucherJobAsync(int voucherId, DateTime scheduledStartDate)
    {
        var voucher = await _voucherRepository.GetByIdAsync(voucherId);
        if (voucher != null)
        {
            if (voucher.StartDate == scheduledStartDate && DateTime.Now >= voucher.StartDate && DateTime.Now < voucher.EndDate)
            {
                voucher.IsActive = true;
                await _voucherRepository.UpdateAsync(voucher);
            }
        }
    }

    public async Task DeactivateVoucherJobAsync(int voucherId, DateTime scheduledEndDate)
    {
        var voucher = await _voucherRepository.GetByIdAsync(voucherId);
        if (voucher != null)
        {
            if (DateTime.Now >= voucher.EndDate || voucher.EndDate == scheduledEndDate)
            {
                voucher.IsActive = false;
                await _voucherRepository.UpdateAsync(voucher);
            }
        }
    }
}
