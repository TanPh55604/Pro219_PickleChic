using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Management;

[Route("management/order-item")]
[ApiController]
public class OrderItemController : ControllerBase
{
    private readonly OrderItemRepository _repository;

    public OrderItemController(OrderItemRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<OrderItem>>> GetAll(string? keyword)
    {
        try
        {
            var result = await _repository.GetAllAsync();
            if (result.Count == 0)
                return NoContent();

            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<OrderItem>> GetById(int id)
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
    public async Task<ActionResult<OrderItem>> Create([FromBody] OrderItemCreateDto dto)
    {
        try
        {
            var entity = new OrderItem
            {
                OrderId = dto.OrderId,
                ProductVariantId = dto.ProductVariantId,
                PromotionId = dto.PromotionId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
                DiscountAmount = dto.DiscountAmount,
                Subtotal = dto.Subtotal,
                IsReviewed = dto.IsReviewed,
                UpdateBy = dto.UpdateBy,
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
    public async Task<ActionResult> Update([FromBody] OrderItemUpdateDto dto)
    {
        try
        {
            var entity = new OrderItem
            {
                Id = dto.Id,
                OrderId = dto.OrderId,
                ProductVariantId = dto.ProductVariantId,
                PromotionId = dto.PromotionId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
                DiscountAmount = dto.DiscountAmount,
                Subtotal = dto.Subtotal,
                IsReviewed = dto.IsReviewed,
                UpdateBy = dto.UpdateBy,
                UpdateAt = DateTime.Now,
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
}
