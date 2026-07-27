using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Management;

[Route("management/review")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly ReviewRepository _repository;

    public ReviewController(ReviewRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<AdminReviewResponseDto>>> GetAll(
        [FromQuery] string? keyword,
        [FromQuery] int? status)
    {
        try
        {
            if (status.HasValue && status is not (1 or 2))
                return BadRequest("Status chỉ nhận 1 (hiện) hoặc 2 (ẩn).");

            var results = await _repository.SearchAsync(keyword, status);
            if (results.Count == 0)
                return NoContent();

            return Ok(results.Select(MapToAdminDto).ToList());
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<AdminReviewResponseDto>> GetById(int id)
    {
        try
        {
            var result = await _repository.GetByIdAsync(id);
            if (result is null)
                return NotFound("Không tìm thấy");

            return Ok(MapToAdminDto(result));
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpPatch("update-status")]
    public async Task<ActionResult<AdminReviewResponseDto>> UpdateStatus([FromBody] ReviewStatusUpdateDto dto)
    {
        try
        {
            if (dto.Status is not (1 or 2))
                return BadRequest("Status chỉ nhận 1 (hiện) hoặc 2 (ẩn).");

            var updated = await _repository.UpdateStatusAsync(dto.Id, dto.Status);
            if (updated is null)
                return NotFound("Không tìm thấy hoặc đã bị xóa");

            var result = await _repository.GetByIdAsync(updated.Id);
            if (result is null)
                return StatusCode(500, "Error loading updated review");

            return Ok(MapToAdminDto(result));
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
                return NotFound("Không tìm thấy");

            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    private static AdminReviewResponseDto MapToAdminDto(Review r) => new()
    {
        Id = r.Id,
        OrderItemId = r.OrderItemId,
        ProductVariantId = r.ProductVariantId,
        Title = r.Title,
        Content = r.Content,
        Overall = r.Overall,
        Status = r.Status,
        CreateAt = r.CreateAt,
        CustomerName = r.OrderItem?.Order?.Customer?.FullName,
        CustomerUsername = r.OrderItem?.Order?.Customer?.Username,
        ProductId = r.ProductVariant?.ProductId,
        ProductName = r.ProductVariant?.Product?.ProductName,
        VariantName = r.ProductVariant?.VariantName,
        SKU = r.ProductVariant?.SKU
    };
}
