using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.API.Services;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Management;

[Route("management/category")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly CategoryRepository _repository;
    private readonly LocalImageFileService _fileService;

    public CategoryController(
        CategoryRepository repository,
        LocalImageFileService fileService)
    {
        _repository = repository;
        _fileService = fileService;
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<Category>>> GetAll(string? keyword)
    {
        try
        {
            var result = await _repository.GetAllAsync();
            
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result
                    .Where(c => c.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase))
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
    public async Task<ActionResult<Category>> GetById(int id)
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
    public async Task<ActionResult<Category>> Create([FromBody] CategoryCreateDto dto)
    {
        try
        {
            var entity = new Category
            {
                Name = dto.Name,
                LinkImage = dto.LinkImage,
                Description = dto.Description,
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

    [HttpPost("upload-image/{id}")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Category>> UploadImage(int id, IFormFile file)
    {
        try
        {
            var category = await _repository.GetByIdAsync(id);
            if (category is null)
            {
                return NotFound("Không tìm thấy thể loại");
            }

            _fileService.DeleteByPublicUrl(category.LinkImage);

            var (_, publicUrl) = await _fileService.SaveCategoryImageAsync(file, id);
            var updated = await _repository.UpdateLinkImageAsync(id, publicUrl);

            if (updated is null)
            {
                return NotFound("Không tìm thấy thể loại");
            }

            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpDelete("delete-image/{id}")]
    public async Task<ActionResult> DeleteImage(int id)
    {
        try
        {
            var category = await _repository.GetByIdAsync(id);
            if (category is null)
            {
                return NotFound("Không tìm thấy thể loại");
            }

            _fileService.DeleteByPublicUrl(category.LinkImage);

            var updated = await _repository.UpdateLinkImageAsync(id, null);
            if (updated is null)
            {
                return NotFound("Không tìm thấy thể loại");
            }

            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpPatch("update")]
    public async Task<ActionResult> Update([FromBody] CategoryUpdateDto dto)
    {
        try
        {
            var entity = new Category
            {
                Id = dto.Id,
                Name = dto.Name,
                LinkImage = dto.LinkImage,
                Description = dto.Description,
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
