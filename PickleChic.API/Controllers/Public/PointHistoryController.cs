using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;

namespace PickleChic.API.Controllers.Public;

[Route("point-history")]
[ApiController]
public class PointHistoryController : ControllerBase
{
    private readonly PointHistoryRepository _repository;

    public PointHistoryController(PointHistoryRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<List<PointHistory>>> GetByCustomerId(int customerId)
    {
        try
        {
            var result = await _repository.GetByCustomerIdAsync(customerId);
            if (result == null || result.Count == 0)
                return NotFound("Không tìm thấy lịch sử điểm của khách hàng này.");

            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }
}
