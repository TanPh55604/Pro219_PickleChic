using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Public;

[Route("product")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly ProductRepository _repository;

    public ProductController(ProductRepository repository)
    {
        _repository = repository;
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
    public async Task<ActionResult<ProductSearchResultDto>> GetByIdWithDetails(int id)
    {
        try
        {
            var p = await _repository.GetProductWithDetailsByIdAsync(id);
            if (p is null)
                return NotFound();

            var dto = new ProductSearchResultDto
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
                ProductVariants = p.ProductVariants?.Select(pv => new ProductVariantFilterDto
                {
                    Id = pv.Id,
                    ProductId = pv.ProductId,
                    SKU = pv.SKU,
                    VariantName = pv.VariantName,
                    Price = pv.Price,
                    StockQuantity = pv.StockQuantity,
                    Status = pv.Status,
                    ProductName = p.ProductName,
                    ProductDescription = p.Description,
                    CategoryName = p.Category?.Name,
                    CategoryDescription = p.Category?.Description,
                    BrandName = p.Brand?.Name,
                    BrandDescription = p.Brand?.Description,
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
                }).ToList() ?? new List<ProductVariantFilterDto>()
            };

            return Ok(dto);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<ProductSearchResultDto>>> Search(
        string? keyword,
        decimal? startingPrice,
        decimal? toPrice,
        string? sortBy,
        int? pageNumber,
        int? pageSize)
    {
        try
        {
            var products = await _repository.SearchProductsWithVariantsAsync(keyword, startingPrice, toPrice, sortBy, pageNumber, pageSize);

            var dtos = products.Select(p =>
            {
                var variantsQuery = p.ProductVariants ?? new List<ProductVariant>();

                if (startingPrice.HasValue)
                {
                    variantsQuery = variantsQuery.Where(pv => pv.Price >= startingPrice.Value).ToList();
                }
                if (toPrice.HasValue)
                {
                    variantsQuery = variantsQuery.Where(pv => pv.Price <= toPrice.Value).ToList();
                }

                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    variantsQuery = sortBy.ToLower() switch
                    {
                        "price_asc" => variantsQuery.OrderBy(pv => pv.Price).ToList(),
                        "price_desc" => variantsQuery.OrderByDescending(pv => pv.Price).ToList(),
                        _ => variantsQuery
                    };
                }

                return new ProductSearchResultDto
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
                    ProductVariants = variantsQuery.Select(pv => new ProductVariantFilterDto
                    {
                        Id = pv.Id,
                        ProductId = pv.ProductId,
                        SKU = pv.SKU,
                        VariantName = pv.VariantName,
                        Price = pv.Price,
                        StockQuantity = pv.StockQuantity,
                        Status = pv.Status,
                        ProductName = p.ProductName,
                        ProductDescription = p.Description,
                        CategoryName = p.Category?.Name,
                        CategoryDescription = p.Category?.Description,
                        BrandName = p.Brand?.Name,
                        BrandDescription = p.Brand?.Description,
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
            }).ToList();

            return Ok(dtos);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }



}
