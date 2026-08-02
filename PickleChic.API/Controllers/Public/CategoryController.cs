using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.API.Services;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace PickleChic.API.Controllers.Public;

[Route("/category")]
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
    public async Task<ActionResult<List<Category>>> GetAll()
    {
        try
        {
            var result = await _repository.GetAllAsync();

            result = result
                    .Where(c => c.Status==1 && c.Delete!=true)
                    .ToList();

            foreach (var category in result)
            {
                category.LinkImage = _fileService.ToAbsolutePublicUrl(category.LinkImage);
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
            if (result is null || result.Status != 1 || result.Delete == true)
                return NotFound("Không tìm thấy");

            result.LinkImage = _fileService.ToAbsolutePublicUrl(result.LinkImage);
            return Ok(result);
        }
        catch (Exception)
        {
            return BadRequest("Lỗi");
        }
    }

}
