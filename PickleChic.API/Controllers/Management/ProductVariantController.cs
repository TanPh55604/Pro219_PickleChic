using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Management;

[Route("management/product-variant")]
[ApiController]
public class ProductVariantController : ControllerBase
{
    private readonly ProductVariantRepository _repository;

    public ProductVariantController(ProductVariantRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<ProductVariant>>> GetAll(string? keyword)
    {
        try
        {
            var result = await _repository.GetAllAsync();
            if (result.Count == 0)
                return NoContent();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result
                    .Where(pv => pv.SKU.Contains(keyword, StringComparison.OrdinalIgnoreCase))
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
    public async Task<ActionResult<ProductVariantDetailDto>> GetByIdWithDetails(int id)
    {
        try
        {
            var pv = await _repository.GetVariantWithDetailsByIdAsync(id);
            if (pv is null)
                return NotFound();

            var dto = new ProductVariantDetailDto
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
            };

            return Ok(dto);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }


    [HttpPost("create")]
    public async Task<ActionResult<ProductVariant>> Create([FromBody] ProductVariantCreateDto dto)
    {
        try
        {
            var entity = new ProductVariant
            {
                ProductId = dto.ProductId,
                SKU = dto.SKU,
                VariantName = dto.VariantName,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                Status = dto.Status,
            };

            var created = await _repository.AddAsync(entity);
            return Ok(created);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpPost("create-with-attributes")]
    public async Task<ActionResult<ProductVariantResponseDto>> CreateWithAttributes([FromBody] ProductVariantWithAttributesCreateDto dto)
    {
        try
        {
            var entity = new ProductVariant
            {
                ProductId = dto.ProductId,
                SKU = dto.SKU,
                VariantName = dto.VariantName,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                Status = dto.Status,
                ProductVariantAttributes = dto.AttributeValueIds?.Select(id => new ProductVariantAttribute
                {
                    AttributeValueId = id
                }).ToList()
            };

            var created = await _repository.AddAsync(entity);
            
            var responseDto = new ProductVariantResponseDto
            {
                Id = created.Id,
                ProductId = created.ProductId,
                SKU = created.SKU,
                VariantName = created.VariantName,
                Price = created.Price,
                StockQuantity = created.StockQuantity,
                Status = created.Status,
                AttributeValueIds = created.ProductVariantAttributes?.Select(pva => pva.AttributeValueId).ToList() ?? new List<int>()
            };

            return Ok(responseDto);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpPatch("update")]
    public async Task<ActionResult> Update([FromBody] ProductVariantUpdateDto dto)
    {
        try
        {
            var entity = new ProductVariant
            {
                Id = dto.Id,
                ProductId = dto.ProductId,
                SKU = dto.SKU,
                VariantName = dto.VariantName,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                Status = dto.Status,
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

    [HttpPatch("update-with-attributes")]
    public async Task<ActionResult<ProductVariantResponseDto>> UpdateWithAttributes([FromBody] ProductVariantWithAttributesUpdateDto dto)
    {
        try
        {
            var entity = new ProductVariant
            {
                Id = dto.Id,
                ProductId = dto.ProductId,
                SKU = dto.SKU,
                VariantName = dto.VariantName,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                Status = dto.Status,
            };

            var updated = await _repository.UpdateWithAttributesAsync(entity, dto.AttributeValueIds);
            if (updated is null)
                return NotFound();

            var responseDto = new ProductVariantResponseDto
            {
                Id = updated.Id,
                ProductId = updated.ProductId,
                SKU = updated.SKU,
                VariantName = updated.VariantName,
                Price = updated.Price,
                StockQuantity = updated.StockQuantity,
                Status = updated.Status,
                AttributeValueIds = updated.ProductVariantAttributes?.Select(pva => pva.AttributeValueId).ToList() ?? new List<int>()
            };

            return Ok(responseDto);
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
