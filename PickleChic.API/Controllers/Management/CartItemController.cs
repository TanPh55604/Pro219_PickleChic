using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.API.Services;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Management;

[Route("management/cart-item")]
[ApiController]
public class CartItemController : ControllerBase
{
    private readonly CartItemRepository _repository;
    private readonly ProductVariantRepository _variantRepository;
    private readonly LocalImageFileService _fileService;

    public CartItemController(
        CartItemRepository repository,
        ProductVariantRepository variantRepository,
        LocalImageFileService fileService)
    {
        _repository = repository;
        _variantRepository = variantRepository;
        _fileService = fileService;
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<CartItem>>> GetAll(string? keyword)
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

    [HttpGet("get-by-user/{userId}")]
    public async Task<ActionResult<List<CartItemDto>>> GetByUserId(int userId)
    {
        try
        {
            var cartItems = await _repository.GetByCustomerIdAsync(userId);
            var result = cartItems.Select(ci => new CartItemDto
            {
                Id = ci.Id,
                CustomerId = ci.CustomerId,
                ProductVariantId = ci.ProductVariantId,
                Quantity = ci.Quantity,
                InsertedAt = ci.InsertedAt,
                ProductVariant = ci.ProductVariant == null ? null : new CartProductVariantDto
                {
                    Id = ci.ProductVariant.Id,
                    ProductId = ci.ProductVariant.ProductId,
                    SKU = ci.ProductVariant.SKU,
                    VariantName = ci.ProductVariant.VariantName,
                    Price = ci.ProductVariant.Price,
                    StockQuantity = ci.ProductVariant.StockQuantity,
                    Status = ci.ProductVariant.Status,
                    ProductName = ci.ProductVariant.Product?.ProductName ?? string.Empty,
                    MainImageUrl = _fileService.ToAbsolutePublicUrl(
                        ci.ProductVariant.ProductVariantImages?
                            .OrderByDescending(img => img.IsMain)
                            .Select(img => img.URL)
                            .FirstOrDefault())
                }
            }).ToList();

            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<CartItem>> GetById(int id)
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
    public async Task<ActionResult<CartItem>> Create([FromBody] CartItemCreateDto dto)
    {
        try
        {
            var variant = await _variantRepository.GetByIdAsync(dto.ProductVariantId);
            if (variant is null || variant.Status == -1)
                return NotFound("Sản phẩm không tồn tại");

            var existingItems = await _repository.GetByCustomerIdAsync(dto.CustomerId);
            var existingItem = existingItems.FirstOrDefault(ci => ci.ProductVariantId == dto.ProductVariantId);

            int targetQuantity = dto.Quantity;
            if (existingItem is not null)
            {
                targetQuantity += existingItem.Quantity;
            }

            if (variant.StockQuantity < targetQuantity)
            {
                return BadRequest(new { message = "Không đủ số lượng yêu cầu" });
            }

            if (existingItem is not null)
            {
                existingItem.Quantity = targetQuantity;
                var updated = await _repository.UpdateAsync(existingItem);
                return Ok(updated);
            }
            else
            {
                var entity = new CartItem
                {
                    CustomerId = dto.CustomerId,
                    ProductVariantId = dto.ProductVariantId,
                    Quantity = dto.Quantity,
                    InsertedAt = DateTime.Now,
                };

                var created = await _repository.AddAsync(entity);
                return Ok(created);
            }
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpPatch("update")]
    public async Task<ActionResult> Update([FromBody] CartItemUpdateDto dto)
    {
        try
        {
            var variant = await _variantRepository.GetByIdAsync(dto.ProductVariantId);
            if (variant is null || variant.Status == -1)
                return NotFound("Sản phẩm không tồn tại");

            if (variant.StockQuantity < dto.Quantity)
            {
                return BadRequest(new { message = "Không đủ số lượng yêu cầu" });
            }

            var entity = new CartItem
            {
                Id = dto.Id,
                CustomerId = dto.CustomerId,
                ProductVariantId = dto.ProductVariantId,
                Quantity = dto.Quantity,
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
