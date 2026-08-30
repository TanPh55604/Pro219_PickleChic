using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;
using System.Threading.Tasks;
using System.Linq;

namespace PickleChic.API.Services;

public class OrderStockService
{
    private readonly OrderRepository _orderRepository;
    private readonly ProductVariantRepository _productVariantRepository;

    public OrderStockService(
        OrderRepository orderRepository,
        ProductVariantRepository productVariantRepository)
    {
        _orderRepository = orderRepository;
        _productVariantRepository = productVariantRepository;
    }

    public async Task<(bool Success, string Message)> DeductStockForOrderAsync(Order order)
    {
        if (order.StockDeducted)
        {
            return (true, "Đã trừ kho.");
        }

        if (order.OrderItems == null || !order.OrderItems.Any())
        {
            order.StockDeducted = true;
            await _orderRepository.UpdateAsync(order);
            return (true, "Không có sản phẩm nào để trừ kho.");
        }

        // 1. Pre-check stock
        foreach (var orderItem in order.OrderItems.Where(oi => !oi.Delete))
        {
            var variant = await _productVariantRepository.GetVariantWithDetailsByIdAsync(orderItem.ProductVariantId);
            if (variant == null)
            {
                return (false, $"Sản phẩm biến thể ID {orderItem.ProductVariantId} không tồn tại.");
            }
            if (variant.StockQuantity < orderItem.Quantity)
            {
                return (false, $"Số lượng kho của biến thể '{variant.VariantName ?? variant.SKU}' không đủ (Yêu cầu: {orderItem.Quantity}, Hiện có: {variant.StockQuantity})");
            }
        }

        // 2. Perform decrease
        foreach (var orderItem in order.OrderItems.Where(oi => !oi.Delete))
        {
            var decreased = await _productVariantRepository.DecreaseStockAsync(orderItem.ProductVariantId, orderItem.Quantity);
            if (!decreased)
            {
                return (false, $"Trừ kho thất bại cho sản phẩm biến thể ID {orderItem.ProductVariantId}");
            }
        }

        order.StockDeducted = true;
        await _orderRepository.UpdateAsync(order);

        return (true, "Trừ kho thành công.");
    }

    public async Task RefundStockForOrderIfDeductedAsync(Order order)
    {
        if (!order.StockDeducted)
        {
            return;
        }

        if (order.OrderItems != null && order.OrderItems.Any())
        {
            foreach (var orderItem in order.OrderItems.Where(oi => !oi.Delete))
            {
                await _productVariantRepository.IncreaseStockAsync(orderItem.ProductVariantId, orderItem.Quantity);
            }
        }

        order.StockDeducted = false;
        await _orderRepository.UpdateAsync(order);
    }
}
