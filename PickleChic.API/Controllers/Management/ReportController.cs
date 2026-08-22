using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PickleChic.API.DTOs;
using PickleChic.DAL.Context;
using PickleChic.DAL.Models;

namespace PickleChic.API.Controllers.Management;

[Route("management/report")]
[ApiController]
public class ReportController : ControllerBase
{
    private readonly PickleChicDbContext _context;

    public ReportController(PickleChicDbContext context)
    {
        _context = context;
    }

    [HttpGet("statistics")]
    public async Task<ActionResult<ReportResponseDto>> GetStatistics(
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] string? groupBy = "month")
    {
        try
        {
            var start = startDate ?? new DateTime(DateTime.Now.Year, 1, 1);
            var end = endDate ?? DateTime.Now;

            if (end.TimeOfDay == TimeSpan.Zero)
            {
                end = end.Date.AddDays(1).AddTicks(-1);
            }

            if (start > end)
            {
                return BadRequest("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
            }

            var selectedGroupBy = string.IsNullOrWhiteSpace(groupBy) ? "month" : groupBy.Trim().ToLower();

            var orders = await _context.Orders
                .Include(o => o.Voucher)
                .Include(o => o.OrderItems!)
                    .ThenInclude(oi => oi.ProductVariant!)
                        .ThenInclude(pv => pv.Product!)
                            .ThenInclude(p => p.Category)
                .Where(o => !o.Delete && o.OrderDate >= start && o.OrderDate <= end)
                .ToListAsync();

            var completedOrders = orders.Where(IsCompletedOrder).ToList();

            var overallOverview = CalculateOverview("Tất cả", start, end, orders);

            var periodBreakdowns = CalculatePeriodBreakdowns(start, end, selectedGroupBy, orders);

            var topProductVariants = CalculateTopProductVariants(completedOrders);

            var topCategories = CalculateTopCategories(completedOrders);

            var result = new ReportResponseDto
            {
                StartDate = start,
                EndDate = end,
                GroupBy = selectedGroupBy,
                OverallOverview = overallOverview,
                PeriodBreakdowns = periodBreakdowns,
                TopProductVariants = topProductVariants,
                TopCategories = topCategories
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    private static bool IsCompletedOrder(Order order)
    {
        if (string.IsNullOrWhiteSpace(order.OrderStatus)) return false;
        var status = order.OrderStatus.Trim();
        return string.Equals(status, "Hoàn thành", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(status, "Done", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsCancelledOrder(Order order)
    {
        if (string.IsNullOrWhiteSpace(order.OrderStatus)) return false;
        var status = order.OrderStatus.Trim();
        return string.Equals(status, "Đã hủy(KH)", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(status, "Đã hủy", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(status, "Giao thất bại", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(status, "Cancelled", StringComparison.OrdinalIgnoreCase);
    }

    private static decimal CalculateOrderNetRevenue(Order order)
    {
        if (order.OrderItems == null || !order.OrderItems.Any()) return 0;

        decimal itemTotal = order.OrderItems
            .Where(oi => !oi.Delete)
            .Sum(oi => oi.Subtotal);

        decimal discountAmount = 0;
        if (order.Voucher != null)
        {
            var voucher = order.Voucher;
            if (voucher.DiscountType.StartsWith("Percent", StringComparison.OrdinalIgnoreCase))
            {
                discountAmount = itemTotal * (voucher.DiscountValue / 100m);
                if (voucher.MaxDiscountAmount.HasValue && discountAmount > voucher.MaxDiscountAmount.Value)
                {
                    discountAmount = voucher.MaxDiscountAmount.Value;
                }
            }
            else if (voucher.DiscountType.StartsWith("Fixed", StringComparison.OrdinalIgnoreCase))
            {
                discountAmount = voucher.DiscountValue;
            }
            discountAmount = Math.Min(discountAmount, itemTotal);
        }

        return Math.Max(0, itemTotal - discountAmount);
    }

    private static PeriodOverviewDto CalculateOverview(string label, DateTime segStart, DateTime segEnd, List<Order> periodOrders)
    {
        var completed = periodOrders.Where(IsCompletedOrder).ToList();
        int totalProductsSold = completed
            .SelectMany(o => o.OrderItems ?? Enumerable.Empty<OrderItem>())
            .Where(oi => !oi.Delete)
            .Sum(oi => oi.Quantity);

        decimal revenue = completed.Sum(CalculateOrderNetRevenue);
        int totalCancelled = periodOrders.Count(IsCancelledOrder);

        return new PeriodOverviewDto
        {
            PeriodLabel = label,
            StartDate = segStart,
            EndDate = segEnd,
            TotalOrders = periodOrders.Count,
            TotalCancelledOrders = totalCancelled,
            TotalProductsSold = totalProductsSold,
            Revenue = revenue
        };
    }

    private static List<PeriodOverviewDto> CalculatePeriodBreakdowns(
        DateTime start,
        DateTime end,
        string groupBy,
        List<Order> orders)
    {
        var breakdowns = new List<PeriodOverviewDto>();
        bool isYear = groupBy == "year" || groupBy == "nam";

        if (isYear)
        {
            for (int year = start.Year; year <= end.Year; year++)
            {
                var yearStart = new DateTime(year, 1, 1);
                var yearEnd = new DateTime(year, 12, 31, 23, 59, 59, 999);

                var segStart = yearStart < start ? start : yearStart;
                var segEnd = yearEnd > end ? end : yearEnd;

                var segOrders = orders
                    .Where(o => o.OrderDate >= segStart && o.OrderDate <= segEnd)
                    .ToList();

                breakdowns.Add(CalculateOverview($"Năm {year}", segStart, segEnd, segOrders));
            }
        }
        else
        {
            var curr = new DateTime(start.Year, start.Month, 1);
            while (curr <= end)
            {
                var monthEnd = curr.AddMonths(1).AddTicks(-1);

                var segStart = curr < start ? start : curr;
                var segEnd = monthEnd > end ? end : monthEnd;

                var segOrders = orders
                    .Where(o => o.OrderDate >= segStart && o.OrderDate <= segEnd)
                    .ToList();

                breakdowns.Add(CalculateOverview($"Tháng {curr:MM/yyyy}", segStart, segEnd, segOrders));

                curr = curr.AddMonths(1);
            }
        }

        return breakdowns;
    }

    private static List<TopProductVariantDto> CalculateTopProductVariants(List<Order> completedOrders)
    {
        var completedOrderItems = completedOrders
            .SelectMany(o => o.OrderItems ?? Enumerable.Empty<OrderItem>())
            .Where(oi => !oi.Delete && oi.ProductVariant != null)
            .ToList();

        return completedOrderItems
            .GroupBy(oi => oi.ProductVariantId)
            .Select(g =>
            {
                var first = g.First();
                var pv = first.ProductVariant!;
                var product = pv.Product;
                var category = product?.Category;

                return new TopProductVariantDto
                {
                    ProductVariantId = g.Key,
                    SKU = pv.SKU ?? string.Empty,
                    VariantName = pv.VariantName,
                    ProductName = product?.ProductName ?? string.Empty,
                    CategoryName = category?.Name ?? "Chưa phân loại",
                    QuantitySold = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.Subtotal)
                };
            })
            .OrderByDescending(v => v.QuantitySold)
            .ThenByDescending(v => v.Revenue)
            .Take(10)
            .ToList();
    }

    private static List<TopCategoryDto> CalculateTopCategories(List<Order> completedOrders)
    {
        var completedOrderItems = completedOrders
            .SelectMany(o => o.OrderItems ?? Enumerable.Empty<OrderItem>())
            .Where(oi => !oi.Delete && oi.ProductVariant?.Product?.Category != null)
            .ToList();

        return completedOrderItems
            .GroupBy(oi => oi.ProductVariant!.Product!.CategoryId)
            .Select(g =>
            {
                var category = g.First().ProductVariant!.Product!.Category!;
                return new TopCategoryDto
                {
                    CategoryId = g.Key,
                    CategoryName = category.Name,
                    TotalProductsSold = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.Subtotal)
                };
            })
            .OrderByDescending(c => c.Revenue)
            .Take(10)
            .ToList();
    }
}
