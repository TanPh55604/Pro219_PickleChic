using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Management;

[Route("management/ward")]
[ApiController]
public class WardController : ControllerBase
{
    private readonly WardRepository _repository;

    public WardController(WardRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<Ward>>> GetAll()
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

    [HttpGet("get-by-district/{districtId}")]
    public async Task<ActionResult<List<Ward>>> GetByDistrict(int districtId)
    {
        try
        {
            var result = await _repository.GetByDistrictIdAsync(districtId);
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
    public async Task<ActionResult<Ward>> GetById(int id)
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
    public async Task<ActionResult<Ward>> Create([FromBody] WardCreateDto dto)
    {
        try
        {
            var entity = new Ward
            {
                Name = dto.Name,
                Code = dto.Code,
                DistrictId = dto.DistrictId,
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
    public async Task<ActionResult> Update([FromBody] WardUpdateDto dto)
    {
        try
        {
            var entity = new Ward
            {
                Id = dto.Id,
                Name = dto.Name,
                Code = dto.Code,
                DistrictId = dto.DistrictId,
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
