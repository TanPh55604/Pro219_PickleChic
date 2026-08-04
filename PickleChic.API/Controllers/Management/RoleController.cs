using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Management;

[Route("management/role")]
[ApiController]
public class RoleController : ControllerBase
{
    private readonly RoleRepository _repository;

    public RoleController(RoleRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<Role>>> GetAll(string? keyword)
    {
        try
        {
            var result = await _repository.GetAllAsync();
            if (result.Count == 0)
                return NoContent();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result
                    .Where(r => r.RoleName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
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
    public async Task<ActionResult<Role>> GetById(int id)
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
    public async Task<ActionResult<Role>> Create([FromBody] RoleCreateDto dto)
    {
        try
        {
            if (await _repository.IsRoleNameExistsAsync(dto.RoleName))
            {
                return BadRequest("Tên vai trò đã tồn tại");
            }

            var entity = new Role
            {
                RoleName = dto.RoleName,
                Permissions = dto.Permissions,
                Status = dto.Status,
                IsEdit = true,
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
    public async Task<ActionResult> Update([FromBody] RoleUpdateDto dto)
    {
        try
        {
            var existing = await _repository.GetByIdAsync(dto.Id);
            if (existing is null)
                return NotFound();

            if (!existing.IsEdit)
            {
                return BadRequest("Vai trò hệ thống không được chỉnh sửa");
            }

            if (await _repository.IsRoleNameExistsAsync(dto.RoleName, dto.Id))
            {
                return BadRequest("Tên vai trò đã tồn tại");
            }

            var entity = new Role
            {
                Id = dto.Id,
                RoleName = dto.RoleName,
                Permissions = dto.Permissions,
                Status = dto.Status,
                IsEdit = existing.IsEdit,
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
            var existing = await _repository.GetByIdAsync(id);
            if (existing is null)
                return NotFound();

            if (!existing.IsEdit)
            {
                return BadRequest("Vai trò hệ thống không được xóa");
            }

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
