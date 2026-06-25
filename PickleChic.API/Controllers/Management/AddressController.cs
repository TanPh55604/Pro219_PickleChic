using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Management;

[Route("management/address")]
[ApiController]
public class AddressController : ControllerBase
{
    private readonly AddressRepository _repository;

    public AddressController(AddressRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<Address>>> GetAll(string? keyword)
    {
        try
        {
            var result = await _repository.GetAllAsync();
            if (result.Count == 0)
                return NoContent();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result
                    .Where(a =>
                        a.FullName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                        || a.PhoneNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                        || a.DetailInfo.Contains(keyword, StringComparison.OrdinalIgnoreCase))
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
    public async Task<ActionResult<Address>> GetById(int id)
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
    public async Task<ActionResult<Address>> Create([FromBody] AddressCreateDto dto)
    {
        try
        {
            var entity = new Address
            {
                CustomerId = dto.CustomerId,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                WardId = dto.WardId,
                DetailInfo = dto.DetailInfo,
                IsDefault = dto.IsDefault,
                Status = dto.Status,
                InsertedAt = DateTime.Now,
                Delete = false
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
    public async Task<ActionResult> Update([FromBody] AddressUpdateDto dto)
    {
        try
        {
            var entity = new Address
            {
                Id = dto.Id,
                CustomerId = dto.CustomerId,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                WardId = dto.WardId,
                DetailInfo = dto.DetailInfo,
                IsDefault = dto.IsDefault,
                Status = dto.Status,
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
