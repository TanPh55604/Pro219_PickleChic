using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;
using System.Security.Claims;

namespace PickleChic.API.Controllers.Public;

[Route("public/voucher")]
[ApiController]
public class VoucherController : ControllerBase
{
    private readonly VoucherRepository _repository;

    public VoucherController(VoucherRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("get-available-voucher")]
    [Authorize]
    public async Task<ActionResult<List<Voucher>>> GetAvailable()
    {
        CustomerRepository _customerRepository = new CustomerRepository();
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.SerialNumber)?.Value);
            var user = await _customerRepository.GetByIdAsync(userId);
            
            if(user!=null)
            {
                var result = await _repository.GetAvailableByRankId(user.RankId);
                return Ok(result);

            } 
            return NotFound();

        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<Voucher>> GetById(int id)
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

}
