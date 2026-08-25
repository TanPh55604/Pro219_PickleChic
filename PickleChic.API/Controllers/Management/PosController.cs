using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Management;

[Route("management/pos")]
[ApiController]
public class PosController : ControllerBase
{
    private readonly ProductVariantRepository _productVariantRepository;
    private readonly CustomerRepository _customerRepository;
    private readonly VoucherRepository _voucherRepository;
    private readonly OrderRepository _orderRepository;

    public PosController(
        ProductVariantRepository productVariantRepository,
        CustomerRepository customerRepository,
        VoucherRepository voucherRepository,
        OrderRepository orderRepository)
    {
        _productVariantRepository = productVariantRepository;
        _customerRepository = customerRepository;
        _voucherRepository = voucherRepository;
        _orderRepository = orderRepository;
    }

    [HttpGet("products")]
    public async Task<ActionResult<ProductVariantSearchPageDto>> SearchProducts(
        [FromQuery] string? keyword,
        [FromQuery] int? brandId,
        [FromQuery] int? categoryId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var page = await _productVariantRepository.SearchForPosAsync(
                keyword, brandId, categoryId, pageNumber, pageSize);

            var items = page.Items.Select(MapVariantToSearchDto).ToList();

            return Ok(new ProductVariantSearchPageDto
            {
                Items = items,
                TotalCount = page.TotalCount,
                PageNumber = page.PageNumber,
                PageSize = page.PageSize
            });
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("products/{variantId:int}/stock")]
    public async Task<ActionResult<PosStockCheckDto>> CheckStock(
        int variantId,
        [FromQuery] int quantity = 1)
    {
        try
        {
            if (variantId <= 0 || quantity <= 0)
            {
                return BadRequest("Biến thể hoặc số lượng không hợp lệ");
            }

            var variant = await _productVariantRepository.GetVariantWithDetailsByIdAsync(variantId);
            if (variant is null
                || variant.Status <= 0
                || variant.Product is null
                || variant.Product.IsDeleted
                || variant.Product.Status <= 0)
            {
                return Ok(new PosStockCheckDto
                {
                    ProductVariantId = variantId,
                    StockQuantity = 0,
                    RequestedQuantity = quantity,
                    IsAvailable = false,
                    Message = "Sản phẩm không còn bán"
                });
            }

            var available = variant.StockQuantity >= quantity;
            return Ok(new PosStockCheckDto
            {
                ProductVariantId = variant.Id,
                StockQuantity = variant.StockQuantity,
                RequestedQuantity = quantity,
                IsAvailable = available,
                UnitPrice = variant.Price,
                ProductName = variant.Product.ProductName,
                VariantName = variant.VariantName,
                Sku = variant.SKU,
                Message = available
                    ? null
                    : $"Chỉ còn {variant.StockQuantity} sản phẩm trong kho"
            });
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("customers")]
    public async Task<ActionResult<CustomerSearchPageDto>> SearchCustomers(
        [FromQuery] string? keyword,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        try
        {
            var (customers, totalCount) = await _customerRepository.SearchForPosAsync(
                keyword, pageNumber, pageSize);

            var safePage = pageNumber < 1 ? 1 : pageNumber;
            var safeSize = pageSize < 1 ? 20 : Math.Min(pageSize, 100);

            return Ok(new CustomerSearchPageDto
            {
                Items = customers.Select(c => new CustomerSearchResultDto
                {
                    Id = c.Id,
                    Username = c.Username,
                    FullName = c.FullName,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber,
                    Gender = c.Gender,
                    DateOfBirth = c.DateOfBirth,
                    TotalPoints = c.TotalPoints,
                    Status = c.Status,
                    RankId = c.RankId,
                    RankName = c.Rank?.RankName,
                    LastLogin = c.LastLogin
                }).ToList(),
                TotalCount = totalCount,
                PageNumber = safePage,
                PageSize = safeSize
            });
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("vouchers")]
    public async Task<ActionResult<List<VoucherUpdateDto>>> GetVouchersByCustomer(
        [FromQuery] int customerId)
    {
        try
        {
            if (customerId <= 0)
                return Ok(new List<VoucherUpdateDto>());

            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer is null || customer.Status <= 0)
                return NotFound("Không tìm thấy khách hàng");

            var spent6Months = await _orderRepository.GetTotalSpentInLast6MonthsAsync(customerId);
            var vouchers = await _voucherRepository.GetAvailableByMinSpend(spent6Months, customer.RankId);

            var result = vouchers.Select(v => new VoucherUpdateDto
            {
                Id = v.Id,
                VoucherCode = v.VoucherCode,
                DiscountType = v.DiscountType,
                DiscountValue = v.DiscountValue,
                MinOrderValue = v.MinOrderValue,
                MaxDiscountAmount = v.MaxDiscountAmount,
                MinimumSpend = v.MinimumSpend,
                StartDate = v.StartDate,
                EndDate = v.EndDate,
                UsageLimit = v.UsageLimit,
                CustomerUsageLimit = v.CustomerUsageLimit,
                UsedCount = v.UsedCount,
                IsActive = v.IsActive,
                RankId = v.RankId,
                IsForever = false
            }).ToList();

            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    private static ProductVariantSearchResultDto MapVariantToSearchDto(ProductVariant pv) => new()
    {
        Id = pv.Id,
        ProductId = pv.ProductId,
        SKU = pv.SKU,
        VariantName = pv.VariantName,
        Price = pv.Price,
        StockQuantity = pv.StockQuantity,
        Status = pv.Status,
        ProductName = pv.Product?.ProductName ?? string.Empty,
        ProductDescription = pv.Product?.Description,
        CategoryName = pv.Product?.Category?.Name,
        CategoryDescription = pv.Product?.Category?.Description,
        BrandName = pv.Product?.Brand?.Name,
        BrandDescription = pv.Product?.Brand?.Description,
        Images = pv.ProductVariantImages?.Select(pvi => new ProductVariantImageDetailDto
        {
            Id = pvi.Id,
            URL = pvi.URL,
            Name = pvi.Name,
            Description = pvi.Description,
            IsMain = pvi.IsMain
        }).ToList() ?? new List<ProductVariantImageDetailDto>(),
        Attributes = pv.ProductVariantAttributes?.Select(pva => new AttributeValueDetailDto
        {
            Id = pva.AttributeValue?.Id ?? 0,
            AttributeId = pva.AttributeValue?.AttributeId ?? 0,
            AttributeName = pva.AttributeValue?.ProductAttribute?.AttributeName ?? string.Empty,
            Value = pva.AttributeValue?.Value ?? string.Empty,
            Note = pva.AttributeValue?.Note
        }).ToList() ?? new List<AttributeValueDetailDto>()
    };
}
