using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;
using Hangfire;
using PickleChic.API.Services;

namespace PickleChic.API.Controllers.Management;

[Route("management/voucher")]
[ApiController]
public class VoucherController : ControllerBase
{
    private readonly VoucherRepository _repository;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public VoucherController(VoucherRepository repository, IBackgroundJobClient backgroundJobClient)
    {
        _repository = repository;
        _backgroundJobClient = backgroundJobClient;
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<Voucher>>> GetAll(string? keyword)
    {
        try
        {
            var result = await _repository.GetAllAsync();
            if (result.Count == 0)
                return NoContent();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result
                    .Where(v => v.VoucherCode.Contains(keyword, StringComparison.OrdinalIgnoreCase))
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
    public async Task<ActionResult<Voucher>> GetById(int id)
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
    public async Task<ActionResult<Voucher>> Create([FromBody] VoucherCreateDto dto)
    {
        try
        {
            if (await _repository.ExistsByCodeAsync(dto.VoucherCode))
                return BadRequest("Mã voucher đã tồn tại");

            var entity = new Voucher
            {
                VoucherCode = dto.VoucherCode.Trim(),
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                MinOrderValue = dto.MinOrderValue,
                MaxDiscountAmount = dto.MaxDiscountAmount,
                MinimumSpend = dto.MinimumSpend,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                UsageLimit = dto.UsageLimit,
                CustomerUsageLimit = dto.CustomerUsageLimit,
                UsedCount = dto.UsedCount,
                IsActive = dto.IsActive,
                RankId = dto.RankId,
            };

            if (dto.IsForever)
            {
                entity.StartDate = DateTime.MinValue;
                entity.EndDate = DateTime.MaxValue;
                entity.UsageLimit = int.MaxValue;
                entity.IsActive = true;
            }
            else
            {
                entity.IsActive = IsWithinActivePeriod(entity.StartDate, entity.EndDate);
            }

            var created = await _repository.AddAsync(entity);

            if (!dto.IsForever)
            {
                if (created.StartDate > DateTime.Now)
                {
                    var startDelay = created.StartDate - DateTime.Now;
                    _backgroundJobClient.Schedule<OrderManagerService>(
                        x => x.ActivateVoucherJobAsync(created.Id, created.StartDate),
                        startDelay
                    );
                }

                if (created.EndDate > DateTime.Now)
                {
                    var endDelay = created.EndDate - DateTime.Now;
                    _backgroundJobClient.Schedule<OrderManagerService>(
                        x => x.DeactivateVoucherJobAsync(created.Id, created.EndDate),
                        endDelay
                    );
                }
            }

            return Ok(created);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpPatch("update")]
    public async Task<ActionResult> Update([FromBody] VoucherUpdateDto dto)
    {
        try
        {
            var existing = await _repository.GetByIdAsync(dto.Id);
            if (existing is null)
                return NotFound();

            if (!CanModifyVoucher(existing.StartDate))
            {
                return BadRequest("Không thể sửa voucher đang diễn ra hoặc đã kết thúc");
            }

            if (await _repository.ExistsByCodeAsync(dto.VoucherCode, dto.Id))
                return BadRequest("Mã voucher đã tồn tại");

            var entity = new Voucher
            {
                Id = dto.Id,
                VoucherCode = dto.VoucherCode.Trim(),
                DiscountType = dto.DiscountType,
                DiscountValue = dto.DiscountValue,
                MinOrderValue = dto.MinOrderValue,
                MaxDiscountAmount = dto.MaxDiscountAmount,
                MinimumSpend = dto.MinimumSpend,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                UsageLimit = dto.UsageLimit,
                CustomerUsageLimit = dto.CustomerUsageLimit,
                UsedCount = dto.UsedCount,
                IsActive = dto.IsActive,
                RankId = dto.RankId,
            };

            if (dto.IsForever)
            {
                entity.StartDate = DateTime.MinValue;
                entity.EndDate = DateTime.MaxValue;
                entity.UsageLimit = int.MaxValue;
                entity.IsActive = true;
            }
            else
            {
                entity.IsActive = IsWithinActivePeriod(entity.StartDate, entity.EndDate);
            }

            var updated = await _repository.UpdateAsync(entity);
            if (updated is null)
                return NotFound();
            if (!dto.IsForever)
            {
                if (updated.StartDate > DateTime.Now)
                {
                    var startDelay = updated.StartDate - DateTime.Now;
                    _backgroundJobClient.Schedule<OrderManagerService>(
                        x => x.ActivateVoucherJobAsync(updated.Id, updated.StartDate),
                        startDelay
                    );
                }

                if (updated.EndDate > DateTime.Now)
                {
                    var endDelay = updated.EndDate - DateTime.Now;
                    _backgroundJobClient.Schedule<OrderManagerService>(
                        x => x.DeactivateVoucherJobAsync(updated.Id, updated.EndDate),
                        endDelay
                    );
                }
            }

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

            if (!CanModifyVoucher(existing.StartDate))
            {
                return BadRequest("Không thể xóa voucher đang diễn ra hoặc đã kết thúc");
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

    private static bool IsWithinActivePeriod(DateTime startDate, DateTime endDate)
    {
        var now = DateTime.Now;
        return startDate <= now && endDate > now;
    }

    private static bool CanModifyVoucher(DateTime startDate)
    {
        return startDate > DateTime.Now;
    }
}
