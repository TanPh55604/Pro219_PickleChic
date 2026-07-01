using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Management;

[Route("management/order")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly OrderRepository _repository;

    public OrderController(OrderRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<Order>>> GetAll(string? keyword)
    {
        try
        {
            var result = await _repository.GetAllAsync();
            if (result.Count == 0)
                return NoContent();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result
                    .Where(o => o.OrderCode.Contains(keyword, StringComparison.OrdinalIgnoreCase))
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
    public async Task<ActionResult<Order>> GetById(int id)
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
    public async Task<ActionResult<Order>> Create([FromBody] OrderCreateDto dto)
    {
        try
        {
            var entity = new Order
            {
                CustomerId = dto.CustomerId,
                OrderCode = dto.OrderCode,
                AddressId = dto.AddressId,
                OrderDate = dto.OrderDate,
                PaymentMethodId = dto.PaymentMethodId,
                VoucherId = dto.VoucherId,
                PaymentStatus = dto.PaymentStatus,
                OrderStatus = dto.OrderStatus,
                Notes = dto.Notes,
                CustomerType = dto.CustomerType,
                IsOrderPOS = dto.IsOrderPOS,
                PaymentLink = dto.PaymentLink,
                PaymentExpiration = dto.PaymentExpiration,
                ShippingFee = dto.ShippingFee,
                StatusHistory = dto.StatusHistory,
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
    public async Task<ActionResult> Update([FromBody] OrderUpdateDto dto)
    {
        try
        {
            var entity = new Order
            {
                Id = dto.Id,
                CustomerId = dto.CustomerId,
                OrderCode = dto.OrderCode,
                AddressId = dto.AddressId,
                OrderDate = dto.OrderDate,
                PaymentMethodId = dto.PaymentMethodId,
                VoucherId = dto.VoucherId,
                PaymentStatus = dto.PaymentStatus,
                OrderStatus = dto.OrderStatus,
                Notes = dto.Notes,
                CustomerType = dto.CustomerType,
                IsOrderPOS = dto.IsOrderPOS,
                PaymentLink = dto.PaymentLink,
                PaymentExpiration = dto.PaymentExpiration,
                ShippingFee = dto.ShippingFee,
                StatusHistory = dto.StatusHistory,
                UpdateBy = dto.UpdateBy,
                LastUpdate = dto.LastUpdate ?? DateTime.Now,
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

    [HttpPatch("update-status/{id}")]
    public async Task<ActionResult> UpdateStatus(int id, [FromBody] OrderStatusUpdateDto dto)
    {
        try
        {
            var existingOrder = await _repository.GetByIdAsync(id);
            if (existingOrder is null)
                return NotFound("Đơn hàng không tồn tại");

            existingOrder.PaymentStatus = dto.PaymentStatus;
            existingOrder.OrderStatus = dto.OrderStatus;
            existingOrder.LastUpdate = DateTime.Now;
            existingOrder.UpdateBy = dto.UpdateBy ?? "Admin";

            var statusHistory = ParseStatusHistory(existingOrder.StatusHistory);
            statusHistory.Add(new StatusHistoryEntry
            {
                Index = statusHistory.Count + 1,
                Status = dto.OrderStatus,
                OrderStatus = dto.OrderStatus,
                PaymentStatus = dto.PaymentStatus,
                DateTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy")
            });
            
            existingOrder.StatusHistory = System.Text.Json.JsonSerializer.Serialize(statusHistory, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            var updated = await _repository.UpdateAsync(existingOrder);
            if (updated is null)
                return BadRequest("Không thể cập nhật trạng thái đơn hàng");

            return Ok(updated);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    private List<StatusHistoryEntry> ParseStatusHistory(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<StatusHistoryEntry>();
        }
        try
        {
            return System.Text.Json.JsonSerializer.Deserialize<List<StatusHistoryEntry>>(json, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                WriteIndented = false
            }) ?? new List<StatusHistoryEntry>();
        }
        catch
        {
            return new List<StatusHistoryEntry>();
        }
    }
}
