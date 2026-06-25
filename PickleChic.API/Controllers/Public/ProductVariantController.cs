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
            if (result is null)
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
            if (pv is null)
                return NotFound();

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
    public async Task<ActionResult<List<ProductVariantSearchResultDto>>> Search(
        string? keyword,
        decimal? startingPrice,
        decimal? toPrice,
        string? sortBy,
        int? pageNumber,
        int? pageSize)// name_asc,name_desc,price_asc,price_desc
    {
        try
        {
            var variants = await _repository.SearchVariantsAsync(keyword, startingPrice, toPrice, sortBy, pageNumber, pageSize);
            var result = variants.Select(pv => new ProductVariantSearchResultDto
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
            }).ToList();

            return Ok(result);
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
            var variants = await _repository.GetVariantsByBrandIdAsync(brandId, startingPrice, toPrice, sortBy);
            var result = variants.Select(pv => new ProductVariantFilterDto
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
            }).ToList();

            return Ok(result);
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
            var variants = await _repository.GetVariantsByCategoryIdAsync(categoryId, startingPrice, toPrice, sortBy);
            var result = variants.Select(pv => new ProductVariantFilterDto
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
            }).ToList();

            return Ok(result);
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
            var variants = await _repository.GetVariantsByAttributeIdAsync(attributeId, startingPrice, toPrice, sortBy);
            var result = variants.Select(pv => new ProductVariantFilterDto
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
            }).ToList();

            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }
}
