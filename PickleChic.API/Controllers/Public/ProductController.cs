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
            if (result is null || result.IsDeleted || result.Status != 1)
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
            var products = (await _repository.GetProductsWithDetailsAsync(keyword))
                .Where(p => !p.IsDeleted
                    && p.Status == 1
                    && p.Category is not null && !p.Category.Delete && p.Category.Status == 1
                    && p.Brand is not null && !p.Brand.Delete && p.Brand.Status == 1)
                .ToList();

            foreach (var product in products)
            {
                if (product.ProductVariants is null)
                {
                    continue;
                }

                product.ProductVariants = product.ProductVariants
                    .Where(pv => pv.Status == 1
                        && (pv.ProductVariantAttributes == null
                            || !pv.ProductVariantAttributes.Any()
                            || pv.ProductVariantAttributes.All(pva =>
                                pva.AttributeValue != null
                                && pva.AttributeValue.ProductAttribute != null)))
                    .ToList();
            }

            products = products.Where(p => p.ProductVariants?.Any() == true).ToList();

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
            if (p is null
                || p.IsDeleted
                || p.Status != 1
                || p.Category is null || p.Category.Delete || p.Category.Status != 1
                || p.Brand is null || p.Brand.Delete || p.Brand.Status != 1)
            {
                return NotFound();
            }

            p.ProductVariants = p.ProductVariants?
                .Where(pv => pv.Status == 1
                    && (pv.ProductVariantAttributes == null
                        || !pv.ProductVariantAttributes.Any()
                        || pv.ProductVariantAttributes.All(pva =>
                            pva.AttributeValue != null
                            && pva.AttributeValue.ProductAttribute != null)))
                .ToList() ?? new List<ProductVariant>();

            if (!p.ProductVariants.Any())
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
                ProductVariants = p.ProductVariants.Select(pv => new ProductVariantFilterDto
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
            var products = (await _repository.SearchProductsWithVariantsAsync(keyword, startingPrice, toPrice, sortBy))
                .Where(p => !p.IsDeleted
                    && p.Status == 1
                    && p.Category is not null && !p.Category.Delete && p.Category.Status == 1
                    && p.Brand is not null && !p.Brand.Delete && p.Brand.Status == 1)
                .ToList();

            foreach (var product in products)
            {
                if (product.ProductVariants is null)
                {
                    continue;
                }

                product.ProductVariants = product.ProductVariants
                    .Where(pv => pv.Status == 1
                        && (pv.ProductVariantAttributes == null
                            || !pv.ProductVariantAttributes.Any()
                            || pv.ProductVariantAttributes.All(pva =>
                                pva.AttributeValue != null
                                && pva.AttributeValue.ProductAttribute != null)))
                    .ToList();
            }

            products = products.Where(p => p.ProductVariants?.Any() == true).ToList();

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
            }).Where(dto => dto.ProductVariants.Any()).ToList();

            return Ok(dtos);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }
}
