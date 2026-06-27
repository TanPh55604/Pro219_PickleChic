using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PickleChic.API.DTOs;
using PickleChic.API.Options;
using PickleChic.API.Services;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Management;

[Route("management/product-variant-image")]
[ApiController]
public class ProductVariantImageController : ControllerBase
{
    private readonly ProductVariantImageRepository _repository;
    private readonly ProductVariantRepository _variantRepository;
    private readonly LocalImageFileService _fileService;
    private readonly FileStorageOptions _fileOptions;

    public ProductVariantImageController(
        ProductVariantImageRepository repository,
        ProductVariantRepository variantRepository,
        LocalImageFileService fileService,
        IOptions<FileStorageOptions> fileOptions)
    {
        _repository = repository;
        _variantRepository = variantRepository;
        _fileService = fileService;
        _fileOptions = fileOptions.Value;
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<ProductVariantImage>>> GetAll(string? keyword)
    {
        try
        {
            var result = await _repository.GetAllAsync();
            if (result.Count == 0)
                return NoContent();
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result
                    .Where(x => (x.Name != null && x.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
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
    public async Task<ActionResult<ProductVariantImage>> GetById(int id)
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

    [HttpGet("get-by-variant/{variantId}")]
    public async Task<ActionResult<List<ProductVariantImage>>> GetByVariant(int variantId)
    {
        try
        {
            var result = await _repository.GetByVariantIdAsync(variantId);
            if (result.Count == 0)
                return NoContent();

            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpPost("upload")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ProductVariantImage>> Upload(
        [FromForm] int productVariantId,
        IFormFile file,
        [FromForm] bool isMain = false,
        [FromForm] string? name = null)
    {
        try
        {
            var variant = await _variantRepository.GetByIdAsync(productVariantId);
            if (variant is null)
                return NotFound("Không tìm thấy biến thể");

            var imageCount = await _repository.CountByVariantIdAsync(productVariantId);
            if (imageCount >= _fileOptions.MaxImagesPerVariant)
                return BadRequest($"Tối đa {_fileOptions.MaxImagesPerVariant} ảnh cho mỗi biến thể");

            var (_, publicUrl) = await _fileService.SaveProductVariantImageAsync(file, variant.ProductId, productVariantId);
            var shouldBeMain = isMain || imageCount == 0;

            if (shouldBeMain)
            {
                await _repository.SetMainAsync(-1, productVariantId);
            }

            var entity = new ProductVariantImage
            {
                ProductVariantId = productVariantId,
                URL = publicUrl,
                Name = string.IsNullOrWhiteSpace(name) ? file.FileName : name.Trim(),
                Description = null,
                IsMain = shouldBeMain
            };

            var created = await _repository.AddAsync(entity);

            if (shouldBeMain)
            {
                await _repository.SetMainAsync(created.Id, productVariantId);
                created.IsMain = true;
            }

            return Ok(created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpPost("create")]
    public async Task<ActionResult<ProductVariantImage>> Create([FromBody] ProductVariantImageCreateDto dto)
    {
        try
        {
            var entity = new ProductVariantImage
            {
                ProductVariantId = dto.ProductVariantId,
                URL = dto.URL,
                Name = dto.Name,
                Description = dto.Description,
                IsMain = dto.IsMain,
            };

            if (dto.IsMain)
            {
                await _repository.SetMainAsync(-1, dto.ProductVariantId);
            }

            var created = await _repository.AddAsync(entity);

            if (dto.IsMain)
            {
                await _repository.SetMainAsync(created.Id, dto.ProductVariantId);
            }

            return Ok(created);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpPatch("update")]
    public async Task<ActionResult> Update([FromBody] ProductVariantImageUpdateDto dto)
    {
        try
        {
            var entity = new ProductVariantImage
            {
                Id = dto.Id,
                ProductVariantId = dto.ProductVariantId,
                URL = dto.URL,
                Name = dto.Name,
                Description = dto.Description,
                IsMain = dto.IsMain,
            };

            if (dto.IsMain)
            {
                await _repository.SetMainAsync(dto.Id, dto.ProductVariantId);
            }

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

    [HttpPatch("set-main/{id}")]
    public async Task<ActionResult<ProductVariantImage>> SetMain(int id)
    {
        try
        {
            var image = await _repository.GetByIdAsync(id);
            if (image is null)
                return NotFound();

            await _repository.SetMainAsync(id, image.ProductVariantId);

            image.IsMain = true;
            return Ok(image);
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
            var image = await _repository.GetByIdAsync(id);
            if (image is null)
                return NotFound();

            _fileService.DeleteByPublicUrl(image.URL);

            var success = await _repository.DeleteAsync(id);
            if (!success)
                return NotFound();

            if (image.IsMain)
            {
                var remaining = await _repository.GetByVariantIdAsync(image.ProductVariantId);
                if (remaining.Count > 0)
                {
                    await _repository.SetMainAsync(remaining[0].Id, image.ProductVariantId);
                }
            }

            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }
}
