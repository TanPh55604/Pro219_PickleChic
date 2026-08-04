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
            if (result is null || result.Delete)
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
            if (await _repository.ExistsByNameAsync(dto.RankName))
                return BadRequest("Tên xếp hạng đã tồn tại");

            if (await _repository.ExistsBySpendAmountAsync(dto.SpendAmount))
                return BadRequest("Mức chi tiêu của xếp hạng đã tồn tại");

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
            if (await _repository.ExistsByNameAsync(dto.RankName, dto.Id))
                return BadRequest("Tên xếp hạng đã tồn tại");

            if (await _repository.ExistsBySpendAmountAsync(dto.SpendAmount, dto.Id))
                return BadRequest("Mức chi tiêu của xếp hạng đã tồn tại");

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

    [HttpGet("percent-reward")]
    public ActionResult<PercentRewardResponseDto> GetPercentReward()
    {
        var percentReward = _config.GetValue<double?>("PercentReward")
            ?? _config.GetValue<double?>("RewardPercent")
            ?? 10.0;

        return Ok(new PercentRewardResponseDto { PercentReward = percentReward });
    }

    [HttpPatch("percent-reward")]
    public async Task<ActionResult<PercentRewardResponseDto>> UpdatePercentReward([FromBody] PercentRewardUpdateDto dto)
    {
        if (dto is null || dto.PercentReward <= 0 || dto.PercentReward >= 100)
        {
            return BadRequest("Tỷ lệ thưởng phải lớn hơn 0 và nhỏ hơn 100");
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
                return NotFound("Không tìm thấy appsettings.json");
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
                return BadRequest("appsettings.json không hợp lệ");
            }

            jsonNode["PercentReward"] = dto.PercentReward;

            var updatedJson = jsonNode.ToJsonString(options);
            await System.IO.File.WriteAllTextAsync(filePath, updatedJson);

            _config["PercentReward"] = dto.PercentReward.ToString(System.Globalization.CultureInfo.InvariantCulture);

            return Ok(new PercentRewardResponseDto { PercentReward = dto.PercentReward });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Không thể cập nhật tỷ lệ thưởng: {ex.Message}");
        }
    }
}
