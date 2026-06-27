using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Public;

[Route("/product-variant")]
[ApiController]
public class ProductVariantController : ControllerBase
{
    private readonly ProductVariantRepository _repository;

    public ProductVariantController(ProductVariantRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<ProductVariant>> GetById(int id)
    {
        try
        {
            var result = await _repository.GetByIdAsync(id);
            if (result is null || result.Status != 1)
                return NotFound();

            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("get-by-id-with-details/{id}")]
    public async Task<ActionResult<ProductVariantSearchResultDto>> GetByIdWithDetails(int id)
    {
        try
        {
            var pv = await _repository.GetVariantWithDetailsByIdAsync(id);
            if (pv is null
                || pv.Status != 1
                || pv.Product is null
                || pv.Product.IsDeleted
                || pv.Product.Status != 1
                || pv.Product.Category is null || pv.Product.Category.Delete || pv.Product.Category.Status != 1
                || pv.Product.Brand is null || pv.Product.Brand.Delete || pv.Product.Brand.Status != 1
                || (pv.ProductVariantAttributes != null
                    && pv.ProductVariantAttributes.Any()
                    && !pv.ProductVariantAttributes.All(pva =>
                        pva.AttributeValue != null
                        && pva.AttributeValue.ProductAttribute != null)))
            {
                return NotFound();
            }

            var dto = new ProductVariantSearchResultDto
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

            return Ok(dto);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<ProductVariantSearchPageDto>> Search(
        string? keyword,
        decimal? startingPrice,
        decimal? toPrice,
        string? sortBy,
        int? pageNumber,
        int? pageSize)
    {
        try
        {
            var resolvedPageNumber = pageNumber.GetValueOrDefault(1);
            var resolvedPageSize = pageSize.GetValueOrDefault(12);

            if (resolvedPageNumber < 1)
            {
                resolvedPageNumber = 1;
            }

            if (resolvedPageSize < 1)
            {
                resolvedPageSize = 1;
            }

            var variants = (await _repository.SearchVariantsAsync(keyword, startingPrice, toPrice, sortBy))
                .Where(pv => pv.Status == 1
                    && pv.Product is not null
                    && !pv.Product.IsDeleted
                    && pv.Product.Status == 1
                    && pv.Product.Category is not null && !pv.Product.Category.Delete && pv.Product.Category.Status == 1
                    && pv.Product.Brand is not null && !pv.Product.Brand.Delete && pv.Product.Brand.Status == 1
                    && (pv.ProductVariantAttributes == null
                        || !pv.ProductVariantAttributes.Any()
                        || pv.ProductVariantAttributes.All(pva =>
                            pva.AttributeValue != null
                            && pva.AttributeValue.ProductAttribute != null)))
                .ToList();

            var totalCount = variants.Count;
            var items = variants
                .Skip((resolvedPageNumber - 1) * resolvedPageSize)
                .Take(resolvedPageSize)
                .Select(pv => new ProductVariantSearchResultDto
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
                })
                .ToList();

            return Ok(new ProductVariantSearchPageDto
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = resolvedPageNumber,
                PageSize = resolvedPageSize
            });
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("get-by-brand/{brandId}")]
    public async Task<ActionResult<List<ProductVariantFilterDto>>> GetByBrand(
        int brandId,
        decimal? startingPrice,
        decimal? toPrice,
        string? sortBy)
    {
        try
        {
            var variants = (await _repository.GetVariantsByBrandIdAsync(brandId, startingPrice, toPrice, sortBy))
                .Where(pv => pv.Status == 1
                    && pv.Product is not null
                    && !pv.Product.IsDeleted
                    && pv.Product.Status == 1
                    && pv.Product.Category is not null && !pv.Product.Category.Delete && pv.Product.Category.Status == 1
                    && pv.Product.Brand is not null && !pv.Product.Brand.Delete && pv.Product.Brand.Status == 1
                    && (pv.ProductVariantAttributes == null
                        || !pv.ProductVariantAttributes.Any()
                        || pv.ProductVariantAttributes.All(pva =>
                            pva.AttributeValue != null
                            && pva.AttributeValue.ProductAttribute != null)))
                .ToList();

            return Ok(variants.Select(pv => new ProductVariantFilterDto
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
            }).ToList());
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("get-by-category/{categoryId}")]
    public async Task<ActionResult<List<ProductVariantFilterDto>>> GetByCategory(
        int categoryId,
        decimal? startingPrice,
        decimal? toPrice,
        string? sortBy)
    {
        try
        {
            var variants = (await _repository.GetVariantsByCategoryIdAsync(categoryId, startingPrice, toPrice, sortBy))
                .Where(pv => pv.Status == 1
                    && pv.Product is not null
                    && !pv.Product.IsDeleted
                    && pv.Product.Status == 1
                    && pv.Product.Category is not null && !pv.Product.Category.Delete && pv.Product.Category.Status == 1
                    && pv.Product.Brand is not null && !pv.Product.Brand.Delete && pv.Product.Brand.Status == 1
                    && (pv.ProductVariantAttributes == null
                        || !pv.ProductVariantAttributes.Any()
                        || pv.ProductVariantAttributes.All(pva =>
                            pva.AttributeValue != null
                            && pva.AttributeValue.ProductAttribute != null)))
                .ToList();

            return Ok(variants.Select(pv => new ProductVariantFilterDto
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
            }).ToList());
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("get-by-attribute/{attributeId}")]
    public async Task<ActionResult<List<ProductVariantFilterDto>>> GetByAttribute(
        int attributeId,
        decimal? startingPrice,
        decimal? toPrice,
        string? sortBy)
    {
        try
        {
            var variants = (await _repository.GetVariantsByAttributeIdAsync(attributeId, startingPrice, toPrice, sortBy))
                .Where(pv => pv.Status == 1
                    && pv.Product is not null
                    && !pv.Product.IsDeleted
                    && pv.Product.Status == 1
                    && pv.Product.Category is not null && !pv.Product.Category.Delete && pv.Product.Category.Status == 1
                    && pv.Product.Brand is not null && !pv.Product.Brand.Delete && pv.Product.Brand.Status == 1
                    && (pv.ProductVariantAttributes == null
                        || !pv.ProductVariantAttributes.Any()
                        || pv.ProductVariantAttributes.All(pva =>
                            pva.AttributeValue != null
                            && pva.AttributeValue.ProductAttribute != null)))
                .ToList();

            return Ok(variants.Select(pv => new ProductVariantFilterDto
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
            }).ToList());
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }
}
