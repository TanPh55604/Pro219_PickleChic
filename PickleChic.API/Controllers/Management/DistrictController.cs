using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Management;

[Route("management/district")]
[ApiController]
public class DistrictController : ControllerBase
{
    private readonly DistrictRepository _repository;

    public DistrictController(DistrictRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<District>>> GetAll()
    {
        try
        {
            var result = await _repository.GetAllAsync();
            if (result.Count == 0)
                return NoContent();

            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("get-by-province/{provinceId}")]
    public async Task<ActionResult<List<District>>> GetByProvince(int provinceId)
    {
        try
        {
            var result = await _repository.GetByProvinceIdAsync(provinceId);
            if (result.Count == 0)
                return NoContent();

            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<District>> GetById(int id)
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
    public async Task<ActionResult<District>> Create([FromBody] DistrictCreateDto dto)
    {
        try
        {
            var entity = new District
            {
                Name = dto.Name,
                Code = dto.Code,
                ProvinceId = dto.ProvinceId,
                InsertedAt = DateTime.Now
            };

            var created = await _repository.AddAsync(entity);
            return Ok(created);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpPut("update")]
    public async Task<ActionResult> Update([FromBody] DistrictUpdateDto dto)
    {
        try
        {
            var entity = new District
            {
                Id = dto.Id,
                Name = dto.Name,
                Code = dto.Code,
                ProvinceId = dto.ProvinceId,
                UpdatedAt = DateTime.Now
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
