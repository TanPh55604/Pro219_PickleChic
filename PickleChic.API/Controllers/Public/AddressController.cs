using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Public;

[Route("/address")]
[ApiController]
public class AddressController : ControllerBase
{
    private readonly AddressRepository _repository;
    private readonly ProvinceRepository _provinceRepository;
    private readonly DistrictRepository _districtRepository;
    private readonly WardRepository _wardRepository;

    public AddressController(
        AddressRepository repository,
        ProvinceRepository provinceRepository,
        DistrictRepository districtRepository,
        WardRepository wardRepository)
    {
        _repository = repository;
        _provinceRepository = provinceRepository;
        _districtRepository = districtRepository;
        _wardRepository = wardRepository;
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
}
