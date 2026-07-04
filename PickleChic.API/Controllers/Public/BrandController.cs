using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace PickleChic.API.Controllers.Public;

[Route("/brand")]
[ApiController]
public class BrandController : ControllerBase
{
    private readonly BrandRepository _repository;

    public BrandController(BrandRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<Brand>>> GetAll()
    {
        try
        {
            var result = await _repository.GetAllAsync();
            if (result.Count == 0)
                return NoContent();
            result = result
                    .Where(b => b.Status==1 && b.Delete!=true)
                    .ToList();
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
            if (result is null || result.Status!=1 || result.Delete==true)
                return NotFound("Không tìm thấy");

            return Ok(result);
        }
        catch (Exception)
        {
            return BadRequest("Lỗi");
        }
    }   
}
