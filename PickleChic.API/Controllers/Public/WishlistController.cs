using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Public;

[Route("wishlist")]
[ApiController]
public class WishlistController : ControllerBase
{
    private readonly WishlistRepository _repository;
    private readonly ProductRepository _productRepository;

    public WishlistController(WishlistRepository repository, ProductRepository productRepository)
    {
        _repository = repository;
        _productRepository = productRepository;
    }

    [HttpGet("get-all-by-userId/{userId}")]
    public async Task<ActionResult<List<ProductSearchResultDto>>> GetAllByUserId(int userId)
    {
        try
        {
            var wishlistItems = await _repository.GetByCustomerIdAsync(userId);
            if (wishlistItems.Count == 0)
                return NoContent();

            var result = new List<ProductSearchResultDto>();
            foreach (var item in wishlistItems)
            {
                var product = await _productRepository.GetProductWithDetailsByIdAsync(item.ProductId);
                if (product is null || product.IsDeleted)
                    continue;

                var activeVariants = product.ProductVariants?
                    .Where(pv => pv.Status != -1)
                    .ToList() ?? new List<ProductVariant>();

                var dto = new ProductSearchResultDto
                {
                    Id = product.Id,
                    ProductName = product.ProductName,
                    Description = product.Description,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category?.Name,
                    BrandId = product.BrandId,
                    BrandName = product.Brand?.Name,
                    Status = product.Status,
                    CreatedAt = product.CreatedAt,
                    UpdatedAt = product.UpdatedAt,
                    UpdatedBy = product.UpdatedBy,
                    ProductVariants = activeVariants.Select(pv => new ProductVariantFilterDto
                    {
                        Id = pv.Id,
                        ProductId = pv.ProductId,
                        SKU = pv.SKU,
                        VariantName = pv.VariantName,
                        Price = pv.Price,
                        StockQuantity = pv.StockQuantity,
                        Status = pv.Status,
                        ProductName = product.ProductName,
                        ProductDescription = product.Description,
                        CategoryName = product.Category?.Name,
                        CategoryDescription = product.Category?.Description,
                        BrandName = product.Brand?.Name,
                        BrandDescription = product.Brand?.Description,
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
                    }).ToList()
                };

                result.Add(dto);
            }

            if (result.Count == 0)
                return NoContent();

            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<ProductSearchResultDto>> GetById(int id)
    {
        try
        {
            var wishlist = await _repository.GetByIdAsync(id);
            if (wishlist is null)
                return NotFound("Mục yêu thích không tồn tại hoặc sản phẩm đã bị xóa");

            var product = await _productRepository.GetProductWithDetailsByIdAsync(wishlist.ProductId);
            if (product is null || product.IsDeleted)
                return NotFound("Sản phẩm không tồn tại hoặc đã bị xóa");

            var activeVariants = product.ProductVariants?
                .Where(pv => pv.Status != -1)
                .ToList() ?? new List<ProductVariant>();

            var dto = new ProductSearchResultDto
            {
                Id = product.Id,
                ProductName = product.ProductName,
                Description = product.Description,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                BrandId = product.BrandId,
                BrandName = product.Brand?.Name,
                Status = product.Status,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt,
                UpdatedBy = product.UpdatedBy,
                ProductVariants = activeVariants.Select(pv => new ProductVariantFilterDto
                {
                    Id = pv.Id,
                    ProductId = pv.ProductId,
                    SKU = pv.SKU,
                    VariantName = pv.VariantName,
                    Price = pv.Price,
                    StockQuantity = pv.StockQuantity,
                    Status = pv.Status,
                    ProductName = product.ProductName,
                    ProductDescription = product.Description,
                    CategoryName = product.Category?.Name,
                    CategoryDescription = product.Category?.Description,
                    BrandName = product.Brand?.Name,
                    BrandDescription = product.Brand?.Description,
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
                }).ToList()
            };

            return Ok(dto);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpPost("create")]
    public async Task<ActionResult<Wishlist>> Create([FromBody] WishlistCreateDto dto)
    {
        try
        {
            var entity = new Wishlist
            {
                CustomerId = dto.CustomerId,
                ProductId = dto.ProductId,
            };

            var created = await _repository.AddAsync(entity);
            return Ok(created);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpPatch("update")]
    public async Task<ActionResult> Update([FromBody] WishlistUpdateDto dto)
    {
        try
        {
            var entity = new Wishlist
            {
                Id = dto.Id,
                CustomerId = dto.CustomerId,
                ProductId = dto.ProductId,
            };

            var updated = await _repository.UpdateAsync(entity);
            if (updated is null)
                return NotFound();

            return Ok(updated);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var success = await _repository.DeleteAsync(id);
            if (!success)
                return NotFound();

            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }
}
