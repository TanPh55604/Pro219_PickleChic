using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Public;

[Route("product-attribute")]
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
            CategoryId = entity.CategoryId,
            AttributeValues = entity.AttributeValues?
                .OrderBy(v => v.Value)
                .Select(v => new AttributeValueDto
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
    public async Task<ActionResult<List<ProductAttributeDto>>> GetAll()
    {
        try
        {
            var result = await _repository.GetAllAsync();
            var dtos = result
                .OrderBy(a => a.AttributeName)
                .Select(MapToDto)
                .ToList();

            return Ok(dtos);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("get-all-by-categoryId")]
    public async Task<ActionResult<List<ProductAttributeDto>>> GetAllByCategory([FromQuery] int categoryId)
    {
        try
        {
            var result = await _repository.GetAllByCategoryId(categoryId);
            var dtos = result
                .OrderBy(a => a.AttributeName)
                .Select(MapToDto)
                .ToList();

            return Ok(dtos);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }
}
