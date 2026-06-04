using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Management;

[Route("management/product")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly ProductRepository _repository;

    public ProductController(ProductRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<Product>>> GetAll(string? keyword)
    {
        try
        {
            var result = await _repository.GetAllAsync();
            if (result.Count == 0)
                return NoContent();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result
                    .Where(p => p.ProductName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        try
        {
            var result = await _repository.GetByIdAsync(id);
            if (result is null)
                return NotFound();

            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("get-all-with-details")]
    public async Task<ActionResult<List<ProductDetailDto>>> GetAllWithDetails(string? keyword)
    {
        try
        {
            var products = await _repository.GetProductsWithDetailsAsync(keyword);
            if (products.Count == 0)
                return NoContent();

            var dtos = products.Select(p => new ProductDetailDto
            {
                Id = p.Id,
                ProductName = p.ProductName,
                Description = p.Description,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                BrandId = p.BrandId,
                BrandName = p.Brand?.Name,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                UpdatedBy = p.UpdatedBy,
                ProductVariants = p.ProductVariants?.Select(pv => new ProductVariantDetailDto
                {
                    Id = pv.Id,
                    ProductId = pv.ProductId,
                    SKU = pv.SKU,
                    VariantName = pv.VariantName,
                    Price = pv.Price,
                    StockQuantity = pv.StockQuantity,
                    Status = pv.Status,
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
                }).ToList() ?? new List<ProductVariantDetailDto>()
            }).ToList();

            return Ok(dtos);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("get-by-id-with-details/{id}")]
    public async Task<ActionResult<ProductDetailDto>> GetByIdWithDetails(int id)
    {
        try
        {
            var p = await _repository.GetProductWithDetailsByIdAsync(id);
            if (p is null)
                return NotFound();

            var dto = new ProductDetailDto
            {
                Id = p.Id,
                ProductName = p.ProductName,
                Description = p.Description,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name,
                BrandId = p.BrandId,
                BrandName = p.Brand?.Name,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                UpdatedBy = p.UpdatedBy,
                ProductVariants = p.ProductVariants?.Select(pv => new ProductVariantDetailDto
                {
                    Id = pv.Id,
                    ProductId = pv.ProductId,
                    SKU = pv.SKU,
                    VariantName = pv.VariantName,
                    Price = pv.Price,
                    StockQuantity = pv.StockQuantity,
                    Status = pv.Status,
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
                }).ToList() ?? new List<ProductVariantDetailDto>()
            };

            return Ok(dto);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }


    [HttpPost("create")]
    public async Task<ActionResult<Product>> Create([FromBody] ProductCreateDto dto)
    {
        try
        {
            var entity = new Product
            {
                ProductName = dto.ProductName,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                BrandId = dto.BrandId,
                Status = dto.Status,
                UpdatedBy = dto.UpdatedBy,
                CreatedAt = DateTime.Now,
                IsDeleted = false,
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
    public async Task<ActionResult> Update([FromBody] ProductUpdateDto dto)
    {
        try
        {
            var entity = new Product
            {
                Id = dto.Id,
                ProductName = dto.ProductName,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                BrandId = dto.BrandId,
                Status = dto.Status,
                UpdatedBy = dto.UpdatedBy,
                UpdatedAt = DateTime.Now,
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
