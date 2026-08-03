using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Management;

[Route("management/page-permission")]
[ApiController]
public class PagePermissionController : ControllerBase
{
    private readonly PagePermissionRepository _repository;

    public PagePermissionController(PagePermissionRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<PagePermission>>> GetAll(string? keyword)
    {
        try
        {
            var result = await _repository.GetAllAsync();
            if (result.Count == 0)
                return NoContent();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result
                    .Where(p => (p.PageCode != null && p.PageCode.Contains(keyword, StringComparison.OrdinalIgnoreCase)) ||
                                (p.PageRoute != null && p.PageRoute.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
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
    public async Task<ActionResult<PagePermission>> GetById(int id)
    {
        try
        {
            var result = await _repository.GetByIdAsync(id);
            if (result is null)
                return NotFound("Không tìm thấy");

            return Ok(result);
        }
        catch (Exception)
        {
            return BadRequest("Lỗi");
        }
    }

    [HttpPost("create")]
    public async Task<ActionResult<PagePermission>> Create([FromBody] PagePermissionCreateDto dto)
    {
        try
        {
            var entity = new PagePermission
            {
                PageCode = dto.PageCode,
                PageRoute = dto.PageRoute,
                AvailablePermissions = dto.AvailablePermissions,
                DefaultPermissions = dto.DefaultPermissions
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
    public async Task<ActionResult> Update([FromBody] PagePermissionUpdateDto dto)
    {
        try
        {
            var entity = new PagePermission
            {
                Id = dto.Id,
                PageCode = dto.PageCode,
                PageRoute = dto.PageRoute,
                AvailablePermissions = dto.AvailablePermissions,
                DefaultPermissions = dto.DefaultPermissions
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
