using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Management;

[Route("management/product-variant-image")]
[ApiController]
public class ProductVariantImageController : ControllerBase
{
    private readonly ProductVariantImageRepository _repository;

    public ProductVariantImageController(ProductVariantImageRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<ProductVariantImage>>> GetAll(string? keyword)
    {
        try
        {
            var result = await _repository.GetAllAsync();
            if (result.Count == 0)
                return NoContent();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result
                    .Where(x => (x.Name != null && x.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
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
    public async Task<ActionResult<ProductVariantImage>> GetById(int id)
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

    [HttpPost("create")]
    public async Task<ActionResult<ProductVariantImage>> Create([FromBody] ProductVariantImageCreateDto dto)
    {
        try
        {
            var entity = new ProductVariantImage
            {
                ProductVariantId = dto.ProductVariantId,
                URL = dto.URL,
                Name = dto.Name,
                Description = dto.Description,
                IsMain = dto.IsMain,
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
    public async Task<ActionResult> Update([FromBody] ProductVariantImageUpdateDto dto)
    {
        try
        {
            var entity = new ProductVariantImage
            {
                Id = dto.Id,
                ProductVariantId = dto.ProductVariantId,
                URL = dto.URL,
                Name = dto.Name,
                Description = dto.Description,
                IsMain = dto.IsMain,
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
