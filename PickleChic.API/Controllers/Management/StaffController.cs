using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.API.Utilities;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Management;

[Route("management/staff")]
[ApiController]
public class StaffController : ControllerBase
{
    private readonly StaffRepository _repository;

    public StaffController(StaffRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<Staff>>> GetAll(string? keyword)
    {
        try
        {
            var result = await _repository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                result = result
                    .Where(s =>
                        s.FullName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                        || s.UserName.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                        || s.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                        || (!string.IsNullOrWhiteSpace(s.PhoneNumber)
                            && s.PhoneNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
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
    public async Task<ActionResult<Staff>> GetById(int id)
    {
        try
        {
            var result = await _repository.GetByIdAsync(id);
            if (result is null)
                return NotFound("Không tìm thấy");

            return Ok(result);
        }
        catch (Exception)
        {
            return BadRequest("Lỗi");
        }
    }

    [HttpPost("create")]
    public async Task<ActionResult<Staff>> Create([FromBody] StaffCreateDto dto)
    {
        try
        {
            var utilityFunc = new UtilityFunc();
            string tempPassword = utilityFunc.GenerateRandomString(8);
            string hashedPassword = utilityFunc.HashPassword(tempPassword);

            var entity = new Staff
            {
                FullName = dto.FullName,
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = hashedPassword,
                RoleId = dto.RoleId,
                Status = dto.Status,
                LastLogin = null,
            };

            var created = await _repository.AddAsync(entity);

            // Gửi email mật khẩu tạm thời
            string subject = "[PickleChic] Mật khẩu tạm thời cho tài khoản quản trị";
            string bodyHTML = $@"
                <h3>Xin chào {entity.FullName},</h3>
                <p>Tài khoản quản trị viên của bạn đã được tạo thành công trên hệ thống PickleChic.</p>
                <p>Dưới đây là thông tin đăng nhập của bạn:</p>
                <ul>
                    <li><strong>Tên đăng nhập / Email:</strong> {entity.UserName} hoặc {entity.Email}</li>
                    <li><strong>Mật khẩu tạm thời:</strong> {tempPassword}</li>
                </ul>
                <p>Vui lòng đăng nhập và đổi mật khẩu ngay trong lần đăng nhập đầu tiên để đảm bảo bảo mật thông tin.</p>
                <br/>
                <p>Trân trọng,<br/>Đội ngũ PickleChic</p>";
            string body = $"Xin chào {entity.FullName},\nTài khoản quản trị viên của bạn đã được tạo thành công trên hệ thống PickleChic.\nTên đăng nhập: {entity.UserName}\nMật khẩu tạm thời: {tempPassword}\nVui lòng đăng nhập và đổi mật khẩu ngay trong lần đăng nhập đầu tiên.\nTrân trọng,\nĐội ngũ PickleChic";

            await utilityFunc.SendEmailToAddress(entity.Email, entity.FullName, subject, body, bodyHTML);

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

    [HttpPatch("update")]
    public async Task<ActionResult> Update([FromBody] StaffUpdateDto dto)
    {
        try
        {
            var entity = new Staff
            {
                Id = dto.Id,
                FullName = dto.FullName,
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                PasswordHash = dto.PasswordHash,
                RoleId = dto.RoleId,
                LastLogin = dto.LastLogin,
                Status = dto.Status,
            };

            var updated = await _repository.UpdateAsync(entity);
            if (updated is null)
                return NotFound();

            return Ok(updated);
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

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var success = await _repository.SoftDeleteAsync(id);
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
