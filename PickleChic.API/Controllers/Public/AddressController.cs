using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;
using System.Text;
using System.Text.Json;

namespace PickleChic.API.Controllers.Public;

[Route("/address")]
[ApiController]
public class AddressController : ControllerBase
{
    private readonly AddressRepository _repository;
    private readonly ProvinceRepository _provinceRepository;
    private readonly DistrictRepository _districtRepository;
    private readonly WardRepository _wardRepository;
    private readonly IConfiguration _configuration;

    public AddressController(
        AddressRepository repository,
        ProvinceRepository provinceRepository,
        DistrictRepository districtRepository,
        WardRepository wardRepository,
        IConfiguration configuration)
    {
        _repository = repository;
        _provinceRepository = provinceRepository;
        _districtRepository = districtRepository;
        _wardRepository = wardRepository;
        _configuration = configuration;
    }   

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<AddressResultDto>> GetById(int id)
    {
        try
        {
            var a = await _repository.GetByIdAsync(id);
            if (a is null)
                return NotFound();

            var dto = new AddressResultDto
            {
                Id = a.Id,
                CustomerId = a.CustomerId,
                FullName = a.FullName,
                PhoneNumber = a.PhoneNumber,
                DetailInfo = a.DetailInfo,
                WardId = a.WardId,
                WardName = a.Ward?.Name,
                WardCode = a.Ward?.Code,
                DistrictId = a.Ward?.DistrictId ?? 0,
                DistrictName = a.Ward?.District?.Name,
                DistrictCode = a.Ward?.District?.Code,
                ProvinceId = a.Ward?.District?.ProvinceId ?? 0,
                ProvinceName = a.Ward?.District?.Province?.Name,
                ProvinceCode = a.Ward?.District?.Province?.Code,
                IsDefault = a.IsDefault,
                Status = a.Status,
                InsertedAt = a.InsertedAt,
                UpdatedAt = a.UpdatedAt
            };

            return Ok(dto);
        }
        catch (Exception)
        {
            return StatusCode(500, "Lỗi hệ thống");
        }
    }

    [HttpGet("get-by-user/{userId}")]
    public async Task<ActionResult<List<AddressResultDto>>> GetByUserId(int userId)
    {
        try
        {
            var addresses = await _repository.GetByCustomerIdAsync(userId);
            var dtos = addresses.Select(a => new AddressResultDto
            {
                Id = a.Id,
                CustomerId = a.CustomerId,
                FullName = a.FullName,
                PhoneNumber = a.PhoneNumber,
                DetailInfo = a.DetailInfo,
                WardId = a.WardId,
                WardName = a.Ward?.Name,
                WardCode = a.Ward?.Code,
                DistrictId = a.Ward?.DistrictId ?? 0,
                DistrictName = a.Ward?.District?.Name,
                DistrictCode = a.Ward?.District?.Code,
                ProvinceId = a.Ward?.District?.ProvinceId ?? 0,
                ProvinceName = a.Ward?.District?.Province?.Name,
                ProvinceCode = a.Ward?.District?.Province?.Code,
                IsDefault = a.IsDefault,
                Status = a.Status,
                InsertedAt = a.InsertedAt,
                UpdatedAt = a.UpdatedAt
            }).ToList();

            return Ok(dtos);
        }
        catch (Exception)
        {
            return StatusCode(500, "Lỗi hệ thống");
        }
    }

    [HttpPost("create")]
    public async Task<ActionResult<Address>> Create([FromBody] AddressCreateDto dto)
    {
        try
        {
            var entity = new Address
            {
                CustomerId = dto.CustomerId,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                WardId = dto.WardId,
                DetailInfo = dto.DetailInfo,
                IsDefault = dto.IsDefault,
                Status = dto.Status,
                InsertedAt = DateTime.Now,
                Delete = false
            };

            var created = await _repository.AddAsync(entity);
            return Ok(created);
        }
        catch (Exception)
        {
            return StatusCode(500, "Lỗi hệ thống");
        }
    }

    [HttpPut("update")]
    public async Task<ActionResult> Update([FromBody] AddressUpdateDto dto)
    {
        try
        {
            var entity = new Address
            {
                Id = dto.Id,
                CustomerId = dto.CustomerId,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                WardId = dto.WardId,
                DetailInfo = dto.DetailInfo,
                IsDefault = dto.IsDefault,
                Status = dto.Status,
                UpdatedAt = DateTime.Now
            };

            var updated = await _repository.UpdateAsync(entity);
            if (updated is null)
                return NotFound();

            return Ok(updated);
        }
        catch (Exception)
        {
            return StatusCode(500, "Lỗi hệ thống");
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
            return StatusCode(500, "Lỗi hệ thống");
        }
    }

    [HttpGet("provinces")]
    public async Task<ActionResult<List<ProvinceResultDto>>> GetProvinces()
    {
        try
        {
            var result = await _provinceRepository.GetAllAsync();
            var dtos = result.Select(p => new ProvinceResultDto
            {
                Id = p.Id,
                Name = p.Name,
                Code = p.Code
            }).ToList();
            return Ok(dtos);
        }
        catch (Exception)
        {
            return StatusCode(500, "Lỗi hệ thống");
        }
    }

    [HttpGet("districts-by-province/{provinceId}")]
    public async Task<ActionResult<List<DistrictResultDto>>> GetDistrictsByProvinceId(int provinceId)
    {
        try
        {
            var result = await _districtRepository.GetByProvinceIdAsync(provinceId);
            var dtos = result.Select(d => new DistrictResultDto
            {
                Id = d.Id,
                Name = d.Name,
                Code = d.Code,
                ProvinceId = d.ProvinceId
            }).ToList();
            return Ok(dtos);
        }
        catch (Exception)
        {
            return StatusCode(500, "Lỗi hệ thống");
        }
    }

    [HttpGet("wards-by-district/{districtId}")]
    public async Task<ActionResult<List<WardResultDto>>> GetWardsByDistrictId(int districtId)
    {
        try
        {
            var result = await _wardRepository.GetByDistrictIdAsync(districtId);
            var dtos = result.Select(w => new WardResultDto
            {
                Id = w.Id,
                Name = w.Name,
                Code = w.Code,
                DistrictId = w.DistrictId
            }).ToList();
            return Ok(dtos);
        }
        catch (Exception)
        {
            return StatusCode(500, "Lỗi hệ thống");
        }
    }

    [HttpGet("wards")]
    public async Task<ActionResult<List<WardResultDto>>> GetWards()
    {
        try
        {
            var result = await _wardRepository.GetAllAsync();
            var dtos = result.Select(w => new WardResultDto
            {
                Id = w.Id,
                Name = w.Name,
                Code = w.Code,
                DistrictId = w.DistrictId
            }).ToList();
            return Ok(dtos);
        }
        catch (Exception)
        {
            return StatusCode(500, "Lỗi hệ thống");
        }
    }

    [HttpPost("CalculateFee")]
    public async Task<IActionResult> CalculateFee(
           string to_district_code,
           string to_ward_code,
           [FromBody] List<FeeItemDTO> items)
    {
        int from_district_id = 3440;
        string from_ward_code = "13007";
        var token = _configuration["GHN:Token"];
        var shopId = _configuration["GHN:ShopId"];
        FeeItemDTO defaultItem = new FeeItemDTO
        {
            Name = "Hàng hóa",
            Quantity = 2,
            Length = 30,
            Width = 40,
            Height = 5,
            Weight = 400
        };
        if (items == null || items.Count == 0)
        {
            items = new List<FeeItemDTO> { defaultItem };
        }

        int totalWeight = 0;

        foreach (var item in items)
        {
            totalWeight += item.Weight * item.Quantity;
        }

        var url = "https://online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/fee";

        var body = new
        {
            service_type_id = 2,
            from_district_id,
            from_ward_code,
            to_district_code,
            to_ward_code,
            length = 30,
            width = 40,
            height = 5,
            weight = totalWeight,
            insurance_value = 0,
            coupon = (string)null,
            items = items
        };

        using var client = new HttpClient();
        if (!string.IsNullOrEmpty(token))
        {
            client.DefaultRequestHeaders.Add("Token", token);
        }
        if (!string.IsNullOrEmpty(shopId))
        {
            client.DefaultRequestHeaders.Add("ShopId", shopId);
        }

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var jsonBody = JsonSerializer.Serialize(body, options);
        using var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        try
        {
            var response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadAsStringAsync();
            return Ok(JsonDocument.Parse(result));
        }
        catch (Exception)
        {
            return StatusCode(500, "Lỗi khi tính phí vận chuyển =)");
        }
    }
}
