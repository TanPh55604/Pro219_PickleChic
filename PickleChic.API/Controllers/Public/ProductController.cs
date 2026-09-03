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
    private readonly ProductVariantRepository _variantRepository;

    public ProductController(
        ProductRepository repository,
        ProductVariantRepository variantRepository)
    {
        _repository = repository;
        _variantRepository = variantRepository;
    }

    [HttpGet("home")]
    public async Task<ActionResult<HomeProductsDto>> GetHomeProducts()
    {
        try
        {
            const int itemLimit = 4;

            var newProducts = await _variantRepository.GetNewestInStockAsync(itemLimit);
            var bestSellingProducts = await _variantRepository.GetBestSellingInStockAsync(itemLimit);

            return Ok(new HomeProductsDto
            {
                NewProducts = newProducts.Select(ToFilterDto).ToList(),
                BestSellingProducts = bestSellingProducts.Select(ToFilterDto).ToList()
            });
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

    [HttpGet("filter")]
    public async Task<ActionResult<ProductFilterPageDto>> FilterProducts([FromQuery] ProductFilterRequestDto request)
    {
        try
        {
            var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
            var pageSize = request.PageSize < 1 ? 12 : Math.Min(request.PageSize, 100);
            var sortBy = request.SortBy?.Trim().ToLowerInvariant();
            var attributeValueIds = request.AttributeValueIds?
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            var attributeValueGroups = attributeValueIds is { Count: > 0 }
                ? await _repository.GroupAttributeValueIdsByAttributeAsync(attributeValueIds)
                : new List<List<int>>();

            var products = await _repository.FilterProductsWithDetailsAsync(
                request.Keyword,
                request.BrandId,
                request.CategoryId,
                request.AttributeId,
                attributeValueIds,
                request.StartingPrice,
                request.ToPrice,
                sortBy);

            foreach (var product in products)
            {
                if (product.ProductVariants is null)
                {
                    continue;
                }

                var variants = product.ProductVariants
                    .Where(pv =>
                        (request.IncludeInactiveVariants || pv.Status == 1)
                        && (pv.ProductVariantAttributes == null
                            || !pv.ProductVariantAttributes.Any()
                            || pv.ProductVariantAttributes.All(pva =>
                                pva.AttributeValue != null
                                && pva.AttributeValue.ProductAttribute != null)))
                    .AsEnumerable();

                if (request.StartingPrice.HasValue)
                {
                    variants = variants.Where(pv => pv.Price >= request.StartingPrice.Value);
                }

                if (request.ToPrice.HasValue)
                {
                    variants = variants.Where(pv => pv.Price <= request.ToPrice.Value);
                }

                if (request.AttributeId.HasValue)
                {
                    variants = variants.Where(pv =>
                        pv.ProductVariantAttributes != null
                        && pv.ProductVariantAttributes.Any(pva =>
                            pva.AttributeValue != null
                            && pva.AttributeValue.AttributeId == request.AttributeId.Value));
                }

                if (attributeValueGroups.Count > 0)
                {
                    variants = variants.Where(pv =>
                        ProductRepository.VariantMatchesAttributeGroups(pv, attributeValueGroups));
                }

                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    variants = sortBy switch
                    {
                        "price_asc" => variants.OrderBy(pv => pv.Price),
                        "price_desc" => variants.OrderByDescending(pv => pv.Price),
                        _ => variants
                    };
                }

                product.ProductVariants = variants.ToList();
            }

            products = products.Where(p => p.ProductVariants?.Any() == true).ToList();

            var variantItems = products
                .SelectMany(p => (p.ProductVariants ?? new List<ProductVariant>())
                    .Select(pv => new ProductVariantFilterDto
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
                    }))
                .ToList();

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                variantItems = sortBy switch
                {
                    "name_asc" => variantItems
                        .OrderBy(v => v.ProductName)
                        .ThenBy(v => v.VariantName)
                        .ToList(),
                    "name_desc" => variantItems
                        .OrderByDescending(v => v.ProductName)
                        .ThenByDescending(v => v.VariantName)
                        .ToList(),
                    "price_asc" => variantItems.OrderBy(v => v.Price).ToList(),
                    "price_desc" => variantItems.OrderByDescending(v => v.Price).ToList(),
                    _ => variantItems
                };
            }

            var totalCount = variantItems.Count;
            var pageItems = variantItems
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new ProductFilterPageDto
            {
                Items = pageItems,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    private static ProductVariantFilterDto ToFilterDto(ProductVariant variant)
    {
        return new ProductVariantFilterDto
        {
            Id = variant.Id,
            ProductId = variant.ProductId,
            SKU = variant.SKU,
            VariantName = variant.VariantName,
            Price = variant.Price,
            StockQuantity = variant.StockQuantity,
            Status = variant.Status,
            ProductName = variant.Product?.ProductName ?? string.Empty,
            ProductDescription = variant.Product?.Description,
            CategoryName = variant.Product?.Category?.Name,
            CategoryDescription = variant.Product?.Category?.Description,
            BrandName = variant.Product?.Brand?.Name,
            BrandDescription = variant.Product?.Brand?.Description,
            Images = variant.ProductVariantImages?.Select(image => new ProductVariantImageDetailDto
            {
                Id = image.Id,
                URL = image.URL,
                Name = image.Name,
                Description = image.Description,
                IsMain = image.IsMain
            }).ToList() ?? new List<ProductVariantImageDetailDto>(),
            Attributes = variant.ProductVariantAttributes?.Select(attribute => new AttributeValueDetailDto
            {
                Id = attribute.AttributeValue?.Id ?? 0,
                AttributeId = attribute.AttributeValue?.AttributeId ?? 0,
                AttributeName = attribute.AttributeValue?.ProductAttribute?.AttributeName ?? string.Empty,
                Value = attribute.AttributeValue?.Value ?? string.Empty,
                Note = attribute.AttributeValue?.Note
            }).ToList() ?? new List<AttributeValueDetailDto>()
        };
    }
}

