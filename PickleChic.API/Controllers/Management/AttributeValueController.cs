using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Management;

[Route("management/attribute-value")]
[ApiController]
public class AttributeValueController : ControllerBase
{
    private readonly AttributeValueRepository _repository;

    public AttributeValueController(AttributeValueRepository repository)
    {
        _repository = repository;
    }

    private static AttributeValueDto MapToDto(AttributeValue entity)
    {
        return new AttributeValueDto
        {
            Id = entity.Id,
            AttributeId = entity.AttributeId,
            Value = entity.Value,
            Note = entity.Note,
            AttributeName = entity.ProductAttribute?.AttributeName
        };
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<AttributeValueDto>>> GetAll(string? keyword)
    {
        try
        {
            var result = await _repository.GetAllAsync();
            if (result.Count == 0)
                return NoContent();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result
                    .Where(av => av.Value.Contains(keyword, StringComparison.OrdinalIgnoreCase))
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
    public async Task<ActionResult<AttributeValueDto>> GetById(int id)
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
    public async Task<ActionResult<AttributeValueDto>> Create([FromBody] AttributeValueCreateDto dto)
    {
        try
        {
            var entity = new AttributeValue
            {
                AttributeId = dto.AttributeId,
                Value = dto.Value,
                Note = dto.Note,
            };

            var created = await _repository.AddAsync(entity);
            // Re-fetch created entity to include the ProductAttribute name if desired
            var fresh = await _repository.GetByIdAsync(created.Id);
            return fresh is null ? Ok(MapToDto(created)) : Ok(MapToDto(fresh));
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpPatch("update")]
    public async Task<ActionResult<AttributeValueDto>> Update([FromBody] AttributeValueUpdateDto dto)
    {
        try
        {
            var entity = new AttributeValue
            {
                Id = dto.Id,
                AttributeId = dto.AttributeId,
                Value = dto.Value,
                Note = dto.Note,
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
