using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Public;

[Route("review")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly ReviewRepository _repository;

    public ReviewController(ReviewRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<ReviewResponseDto>>> GetAll()
    {
        try
        {
            var results = await _repository.GetAllAsync();
            if (results.Count == 0)
                return NoContent();

            return Ok(results.Select(MapToDto).ToList());
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<ReviewResponseDto>> GetById(int id)
    {
        try
        {
            var result = await _repository.GetByIdAsync(id);
            if (result is null)
                return NotFound("Không tìm thấy");

            return Ok(MapToDto(result));
        }
        catch (Exception)
        {
            return BadRequest("Lỗi");
        }
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult<ReviewResponseDto>> Create([FromBody] ReviewCreateDto dto)
    {
        try
        {
            if (!TryGetCustomerId(out var customerId))
                return Unauthorized("Không thể xác định thông tin người dùng từ token.");

            if (dto.ProductVariantId <= 0)
                return BadRequest("Sản phẩm không hợp lệ.");

            if (dto.Overall < 1 || dto.Overall > 5)
                return BadRequest("Điểm đánh giá phải từ 1 đến 5.");

            if (string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest("Nội dung đánh giá không được để trống.");

            var alreadyReviewed = await _repository.HasCustomerReviewedVariantAsync(customerId, dto.ProductVariantId);
            if (alreadyReviewed)
                return BadRequest("Bạn đã đánh giá sản phẩm này trước đó.");

            var eligibleItem = await _repository.GetEligibleOrderItemAsync(customerId, dto.ProductVariantId);
            if (eligibleItem is null)
                return BadRequest("Bạn chưa mua sản phẩm này hoặc đơn hàng chưa hoàn thành.");

            var entity = new Review
            {
                OrderItemId = eligibleItem.Id,
                ProductVariantId = dto.ProductVariantId,
                Title = string.IsNullOrWhiteSpace(dto.Title) ? null : dto.Title.Trim(),
                Content = dto.Content.Trim(),
                Overall = dto.Overall,
                Status = 1,
                CreateAt = DateTime.Now,
                Delete = false
            };

            var created = await _repository.AddAsync(entity);
            var result = await _repository.GetByIdAsync(created.Id);
            if (result == null)
                return StatusCode(500, "Error loading created review details");

            return Ok(MapToDto(result));
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [Authorize]
    [HttpPatch("update")]
    public async Task<ActionResult<ReviewResponseDto>> Update([FromBody] ReviewUpdateDto dto)
    {
        try
        {
            if (!TryGetCustomerId(out var customerId))
                return Unauthorized("Không thể xác định thông tin người dùng từ token.");

            var existing = await _repository.GetByIdAsync(dto.Id);
            if (existing is null)
                return NotFound("Không tìm thấy hoặc đã bị xóa");

            if (existing.OrderItem?.Order?.CustomerId != customerId)
                return Forbid();

            var entity = new Review
            {
                Id = dto.Id,
                Title = dto.Title,
                Content = dto.Content,
                Overall = dto.Overall,
                Status = existing.Status
            };

            var updated = await _repository.UpdateAsync(entity);
            if (updated is null)
                return NotFound("Không tìm thấy hoặc đã bị xóa");

            var result = await _repository.GetByIdAsync(updated.Id);
            if (result == null)
                return StatusCode(500, "Error loading updated review details");

            return Ok(MapToDto(result));
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [Authorize]
    [HttpDelete("delete/{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            if (!TryGetCustomerId(out var customerId))
                return Unauthorized("Không thể xác định thông tin người dùng từ token.");

            var existing = await _repository.GetByIdAsync(id);
            if (existing is null)
                return NotFound("Không tìm thấy");

            if (existing.OrderItem?.Order?.CustomerId != customerId)
                return Forbid();

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

    [HttpGet("variant/{productVariantId}")]
    public async Task<ActionResult<List<ReviewResponseDto>>> GetReviewsByProductVariantId(int productVariantId)
    {
        try
        {
            var results = await _repository.GetReviewsByProductVariantIdAsync(productVariantId);
            return Ok(results.Select(MapToDto).ToList());
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [AllowAnonymous]
    [HttpGet("variant/{productVariantId}/eligibility")]
    public async Task<ActionResult<ReviewEligibilityDto>> GetEligibility(int productVariantId)
    {
        try
        {
            if (!TryGetCustomerId(out var customerId))
            {
                return Ok(new ReviewEligibilityDto
                {
                    IsAuthenticated = false,
                    CanReview = false,
                    AlreadyReviewed = false,
                    HasCompletedPurchase = false,
                    LockReason = "login"
                });
            }

            var myReview = await _repository.GetCustomerReviewForVariantAsync(customerId, productVariantId);
            if (myReview is not null)
            {
                var myState = myReview.Delete
                    ? "deleted"
                    : myReview.Status == 2
                        ? "hidden"
                        : "visible";

                return Ok(new ReviewEligibilityDto
                {
                    IsAuthenticated = true,
                    CanReview = false,
                    AlreadyReviewed = true,
                    HasCompletedPurchase = true,
                    LockReason = "already_reviewed",
                    MyReviewState = myState
                });
            }

            var eligibleItem = await _repository.GetEligibleOrderItemAsync(customerId, productVariantId);
            var hasPurchase = eligibleItem is not null;

            return Ok(new ReviewEligibilityDto
            {
                IsAuthenticated = true,
                CanReview = hasPurchase,
                AlreadyReviewed = false,
                HasCompletedPurchase = hasPurchase,
                LockReason = hasPurchase ? "none" : "not_purchased",
                MyReviewState = null
            });
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [Authorize]
    [HttpGet("customer/unreviewed")]
    public async Task<ActionResult<List<UnreviewedProductVariantDto>>> GetUnreviewedProductVariants()
    {
        try
        {
            if (!TryGetCustomerId(out var customerId))
                return Unauthorized("Không thể xác định thông tin người dùng từ token.");

            var items = await _repository.GetUnreviewedItemsByCustomerIdAsync(customerId);
            var dtos = items.Select(oi => new UnreviewedProductVariantDto
            {
                OrderItemId = oi.Id,
                ProductVariantId = oi.ProductVariantId,
                SKU = oi.ProductVariant?.SKU,
                VariantName = oi.ProductVariant?.VariantName,
                Price = oi.ProductVariant?.Price ?? 0,
                ProductId = oi.ProductVariant?.ProductId ?? 0,
                ProductName = oi.ProductVariant?.Product?.ProductName,
                ProductImage = oi.ProductVariant?.ProductVariantImages?.FirstOrDefault(pvi => pvi.IsMain)?.URL
                               ?? oi.ProductVariant?.ProductVariantImages?.FirstOrDefault()?.URL
            }).ToList();

            return Ok(dtos);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    private bool TryGetCustomerId(out int customerId)
    {
        customerId = 0;
        var claimVal = User.FindFirst(ClaimTypes.SerialNumber)?.Value;
        return !string.IsNullOrEmpty(claimVal) && int.TryParse(claimVal, out customerId);
    }

    private static ReviewResponseDto MapToDto(Review r) => new()
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
        CustomerUsername = r.OrderItem?.Order?.Customer?.Username
    };
}
