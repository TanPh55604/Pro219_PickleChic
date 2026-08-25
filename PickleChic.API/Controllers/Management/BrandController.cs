using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Management;

[Route("management/brand")]
[ApiController]
public class BrandController : ControllerBase
{
    private readonly BrandRepository _repository;
    private readonly ProductRepository _productRepository;

    public BrandController(BrandRepository repository, ProductRepository productRepository)
    {
        _repository = repository;
        _productRepository = productRepository;
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<Brand>>> GetAll(string? keyword)
    {
        try
        {
            var result = await _repository.GetAllAsync();
            if (result.Count == 0)
                return NoContent();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result
                    .Where(b => b.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
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
    public async Task<ActionResult<Brand>> GetById(int id)
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
    public async Task<ActionResult<Brand>> Create([FromBody] BrandCreateDto dto)
    {
        try
        {
            if (await _repository.ExistsByNameAsync(dto.Name))
                return BadRequest("Tên thương hiệu đã tồn tại");

            var entity = new Brand
            {
                Name = dto.Name,
                Description = dto.Description,
                UpdateBy = dto.UpdateBy,
                Status = dto.Status,
                InsertedAt = DateTime.Now,
                Delete = false,
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
    public async Task<ActionResult> Update([FromBody] BrandUpdateDto dto)
    {
        try
        {
            if (await _repository.ExistsByNameAsync(dto.Name, dto.Id))
                return BadRequest("Tên thương hiệu đã tồn tại");

            if (dto.Status != 1)
            {
                if (await _productRepository.HasActiveProductsByBrandIdAsync(dto.Id))
                {
                    return BadRequest("Không thể vô hiệu hóa thương hiệu này vì vẫn còn sản phẩm đang hoạt động.");
                }
            }

            var entity = new Brand
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                UpdateBy = dto.UpdateBy,
                Status = dto.Status,
                UpdatedAt = DateTime.Now,
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
            if (await _productRepository.HasActiveProductsByBrandIdAsync(id))
            {
                return BadRequest("Không thể xóa thương hiệu này vì vẫn còn sản phẩm đang hoạt động.");
            }

            var success = await _repository.SoftDeleteAsync(id);
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
