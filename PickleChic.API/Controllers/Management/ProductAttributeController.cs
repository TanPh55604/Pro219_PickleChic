using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;
using System.Threading.Tasks;

namespace PickleChic.API.Controllers.Management;

[Route("management/product-attribute")]
[ApiController]
public class ProductAttributeController : ControllerBase
{
    private readonly ProductAttributeRepository _repository;

    public ProductAttributeController(ProductAttributeRepository repository)
    {
        _repository = repository;
    }

    private static ProductAttributeDto MapToDto(ProductAttribute entity)
    {
        return new ProductAttributeDto
        {
            Id = entity.Id,
            AttributeName = entity.AttributeName,
            AttributeValues = entity.AttributeValues?.Select(v => new AttributeValueDto
            {
                Id = v.Id,
                AttributeId = v.AttributeId,
                Value = v.Value,
                Note = v.Note,
                AttributeName = entity.AttributeName
            }).ToList()
        };
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<ProductAttributeDto>>> GetAll(string? keyword)
    {
        try
        {
            var result = await _repository.GetAllAsync();
            if (result.Count == 0)
                return NoContent();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result
                    .Where(a => a.AttributeName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var dtos = result.Select(MapToDto).ToList();
            return Ok(dtos);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<ProductAttributeDto>> GetById(int id)
    {
        try
        {
            var result = await _repository.GetByIdAsync(id);
            if (result is null)
                return NotFound();

            return Ok(MapToDto(result));
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpPost("create")]
    public async Task<ActionResult<ProductAttributeDto>> Create([FromBody] ProductAttributeCreateDto dto)
    {
        try
        {
            var entity = new ProductAttribute
            {
                AttributeName = dto.AttributeName,
            };

            var created = await _repository.AddAsync(entity);
            return Ok(MapToDto(created));
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpPost("create-with-values")]
    public async Task<ActionResult<ProductAttributeDto>> CreateWithValues([FromBody] ProductAttributeWithValuesCreateDto dto)
    {
        try
        {
            var entity = new ProductAttribute
            {
                AttributeName = dto.AttributeName,
                AttributeValues = dto.AttributeValues.Select(v => new AttributeValue
                {
                    Value = v.Value,
                    Note = v.Note
                }).ToList()
            };
            var created = await _repository.AddAsync(entity);
            return Ok(MapToDto(created));
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpPost("modify-with-flag")]
    public async Task<ActionResult<ProductAttributeDto>> ModifyWithValuesAndFlag([FromBody] ProductAttributeModifyWithFlagDto dto)
    {
        try
        {
            var valuesTuple = dto.AttributeValues.Select(v => (v.Id, v.Value, v.Note, v.FlagAction)).ToList();
            var updated = await _repository.UpdateWithValuesAndFlagAsync(dto.Id, dto.AttributeName, valuesTuple);
            if (updated is null)
                return NotFound();

            return Ok(MapToDto(updated));
        }
        catch (Exception)
        {
            return StatusCode(500, "Lỗi hệ thống, vui lòng liên hệ quản trị");
        }
    }

    [HttpPatch("update")]
    public async Task<ActionResult<ProductAttributeDto>> Update([FromBody] ProductAttributeUpdateDto dto)
    {
        try
        {
            var entity = new ProductAttribute
            {
                Id = dto.Id,
                AttributeName = dto.AttributeName,
            };

            var updated = await _repository.UpdateAsync(entity);
            if (updated is null)
                return NotFound();

            var fresh = await _repository.GetByIdAsync(updated.Id);
            return fresh is null ? NotFound() : Ok(MapToDto(fresh));
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
