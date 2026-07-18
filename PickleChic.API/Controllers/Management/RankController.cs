using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PickleChic.API.Controllers.Management;

[Route("management/rank")]
[ApiController]
public class RankController : ControllerBase
{
    private readonly RankRepository _repository;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;

    public RankController(RankRepository repository, IWebHostEnvironment env, IConfiguration config)
    {
        _repository = repository;
        _env = env;
        _config = config;
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<Rank>>> GetAll(string? keyword)
    {
        try
        {
            var result = await _repository.GetAllAsync();
            if (result.Count == 0)
                return NoContent();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result
                    .Where(r => r.RankName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
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
    public async Task<ActionResult<Rank>> GetById(int id)
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
    public async Task<ActionResult<Rank>> Create([FromBody] RankCreateDto dto)
    {
        try
        {
            var entity = new Rank
            {
                RankName = dto.RankName,
                SpendAmount = dto.SpendAmount,
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
    public async Task<ActionResult> Update([FromBody] RankUpdateDto dto)
    {
        try
        {
            var entity = new Rank
            {
                Id = dto.Id,
                RankName = dto.RankName,
                SpendAmount = dto.SpendAmount,
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

    [HttpPatch("percent-reward")]
    public async Task<IActionResult> UpdatePercentReward([FromQuery] double value)
    {
        if (value <= 0 || value >= 100)
        {
            return BadRequest("Value must be greater than 0 and less than 100");
        }

        try
        {
            var filePath = Path.Combine(_env.ContentRootPath, "appsettings.json");
            if (!System.IO.File.Exists(filePath))
            {
                filePath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            }

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("appsettings.json not found");
            }

            var json = await System.IO.File.ReadAllTextAsync(filePath);
            var options = new JsonSerializerOptions 
            { 
                ReadCommentHandling = JsonCommentHandling.Skip, 
                WriteIndented = true 
            };
            var jsonNode = JsonNode.Parse(json);
            if (jsonNode == null)
            {
                return BadRequest("Invalid JSON in appsettings.json");
            }

            jsonNode["PercentReward"] = value;

            var updatedJson = jsonNode.ToJsonString(options);
            await System.IO.File.WriteAllTextAsync(filePath, updatedJson);

            _config["PercentReward"] = value.ToString();

            return Ok(new { PercentReward = value });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error updating appsettings.json: {ex.Message}");
        }
    }
}
