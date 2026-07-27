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

            var dtos = results.Select(r => new ReviewResponseDto
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
            }).ToList();

            return Ok(dtos);
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

            var dto = new ReviewResponseDto
            {
                Id = result.Id,
                OrderItemId = result.OrderItemId,
                ProductVariantId = result.ProductVariantId,
                Title = result.Title,
                Content = result.Content,
                Overall = result.Overall,
                Status = result.Status,
                CreateAt = result.CreateAt,
                CustomerName = result.OrderItem?.Order?.Customer?.FullName,
                CustomerUsername = result.OrderItem?.Order?.Customer?.Username
            };

            return Ok(dto);
        }
        catch (Exception)
        {
            return BadRequest("Lỗi");
        }
    }

    [HttpPost("create")]
    public async Task<ActionResult<ReviewResponseDto>> Create([FromBody] ReviewCreateDto dto)
    {
        try
        {
            var alreadyReviewed = await _repository.HasCustomerReviewedVariantAsync(dto.CustomerId, dto.ProductVariantId);
            if (alreadyReviewed)
            {
                return BadRequest("Bạn đã đánh giá sản phẩm này trước đó.");
            }

            var eligibleItem = await _repository.GetEligibleOrderItemAsync(dto.CustomerId, dto.ProductVariantId);
            if (eligibleItem is null)
            {
                return BadRequest("Bạn chưa mua sản phẩm này hoặc đơn hàng chưa hoàn thành.");
            }

            var entity = new Review
            {
                OrderItemId = eligibleItem.Id,
                ProductVariantId = dto.ProductVariantId,
                Title = dto.Title,
                Content = dto.Content,
                Overall = dto.Overall,
                Status = dto.Status,
                CreateAt = DateTime.Now,
                Delete = false
            };

            var created = await _repository.AddAsync(entity);
            var result = await _repository.GetByIdAsync(created.Id);
            if (result == null)
                return StatusCode(500, "Error loading created review details");

            var responseDto = new ReviewResponseDto
            {
                Id = result.Id,
                OrderItemId = result.OrderItemId,
                ProductVariantId = result.ProductVariantId,
                Title = result.Title,
                Content = result.Content,
                Overall = result.Overall,
                Status = result.Status,
                CreateAt = result.CreateAt,
                CustomerName = result.OrderItem?.Order?.Customer?.FullName,
                CustomerUsername = result.OrderItem?.Order?.Customer?.Username
            };

            return Ok(responseDto);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpPatch("update")]
    public async Task<ActionResult<ReviewResponseDto>> Update([FromBody] ReviewUpdateDto dto)
    {
        try
        {
            var entity = new Review
            {
                Id = dto.Id,
                Title = dto.Title,
                Content = dto.Content,
                Overall = dto.Overall,
                Status = dto.Status
            };

            var updated = await _repository.UpdateAsync(entity);
            if (updated is null)
                return NotFound("Không tìm thấy hoặc đã bị xóa");
            var result = await _repository.GetByIdAsync(updated.Id);
            if (result == null)
                return StatusCode(500, "Error loading updated review details");

            var responseDto = new ReviewResponseDto
            {
                Id = result.Id,
                OrderItemId = result.OrderItemId,
                ProductVariantId = result.ProductVariantId,
                Title = result.Title,
                Content = result.Content,
                Overall = result.Overall,
                Status = result.Status,
                CreateAt = result.CreateAt,
                CustomerName = result.OrderItem?.Order?.Customer?.FullName,
                CustomerUsername = result.OrderItem?.Order?.Customer?.Username
            };

            return Ok(responseDto);
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

    [HttpGet("variant/{productVariantId}")]
    public async Task<ActionResult<List<ReviewResponseDto>>> GetReviewsByProductVariantId(int productVariantId)
    {
        try
        {
            var results = await _repository.GetReviewsByProductVariantIdAsync(productVariantId);
            var dtos = results.Select(r => new ReviewResponseDto
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
            }).ToList();

            return Ok(dtos);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("customer/{customerId}/unreviewed")]
    public async Task<ActionResult<List<UnreviewedProductVariantDto>>> GetUnreviewedProductVariants(int customerId)
    {
        try
        {
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
}
