using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;
using System.Security.Claims;

namespace PickleChic.API.Controllers.Public;

[Route("voucher")]
[ApiController]
public class VoucherController : ControllerBase
{
    private readonly VoucherRepository _repository;
    private readonly OrderRepository _orderRepository;

    public VoucherController(VoucherRepository repository, OrderRepository orderRepository)
    {
        _repository = repository;
        _orderRepository = orderRepository;
    }

    [HttpGet("get-available-voucher")]
    [Authorize]
    public async Task<ActionResult<List<Voucher>>> GetAvailable()
    {
        CustomerRepository _customerRepository = new CustomerRepository();
        RankRepository rankRepository = new RankRepository();
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.SerialNumber)?.Value);
            var user = await _customerRepository.GetByIdAsync(userId);
            
            if(user!=null)
            {
                decimal rankMiniumSpend = (await rankRepository.GetByIdAsync(user.RankId))?.SpendAmount ?? 0;
                var result = await _repository.GetAvailableByMinSpend(rankMiniumSpend, user.RankId);
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
