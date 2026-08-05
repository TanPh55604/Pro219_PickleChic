using Microsoft.AspNetCore.Mvc;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;
using PickleChic.API.Utilities;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace PickleChic.API.Controllers.Management;

[Route("management/order")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly OrderRepository _repository;
    private readonly ProductVariantRepository _productVariantRepository;
    private readonly PointHistoryRepository _pointHistoryRepository;
    private readonly CustomerRepository _customerRepository;
    private readonly RankRepository _rankRepository;
    private readonly IConfiguration _configuration;
    private readonly VoucherRepository _voucherRepository;

    public OrderController(
        OrderRepository repository, 
        ProductVariantRepository productVariantRepository,
        PointHistoryRepository pointHistoryRepository,
        CustomerRepository customerRepository,
        RankRepository rankRepository,
        IConfiguration configuration,
        VoucherRepository voucherRepository)
    {
        _repository = repository;
        _productVariantRepository = productVariantRepository;
        _pointHistoryRepository = pointHistoryRepository;
        _customerRepository = customerRepository;
        _rankRepository = rankRepository;
        _configuration = configuration;
        _voucherRepository = voucherRepository;
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<ManagementOrderResponseDto>>> GetAll(string? keyword, int? status = null)
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
            if(status != null)
            {
                result = result
                    .Where(o => o.Status == status)
                    .ToList();
            }

            return Ok(result.Select(MapToManagementOrderDto).ToList());
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("get-all-bopis")]
    public async Task<ActionResult<List<ManagementOrderResponseDto>>> GetAllBOPIS()
    {
        try
        {
            var result = await _repository.GetAllAsync();
            if (result.Count == 0)
                return NoContent();
           result = result.Where(o => o.BOPIS == true&&o.Delete!=true).OrderByDescending(x=>x.OrderDate).ToList();
            return Ok(result.Select(MapToManagementOrderDto).ToList());
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<ManagementOrderResponseDto>> GetById(int id)
    {
        try
        {
            var result = await _repository.GetByIdAsync(id);
            if (result is null)
                return NotFound();

            return Ok(MapToManagementOrderDto(result));
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpPost("create")]
    public async Task<ActionResult<ManagementOrderResponseDto>> Create([FromBody] OrderCreateDto dto)
    {
        try
        {
            var updatedBy = dto.UpdateBy;
            if (string.IsNullOrEmpty(updatedBy))
            {
                updatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            if (string.IsNullOrEmpty(updatedBy))
            {
                updatedBy = "System";
            }

            var statusHistory = ParseStatusHistory(dto.StatusHistory);
            statusHistory.Add(new StatusHistoryEntry
            {
                Index = statusHistory.Count + 1,
                Status = dto.OrderStatus,
                OrderStatus = dto.OrderStatus,
                PaymentStatus = dto.PaymentStatus,
                DateTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy"),
                UpdatedBy = updatedBy,
                Reasons = "Tạo đơn"
            });

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
                Status = Constant.OrderStatus.GetStatusInt(dto.OrderStatus),
                Notes = dto.Notes,
                CustomerType = dto.CustomerType,
                IsOrderPOS = dto.IsOrderPOS,
                PaymentLink = dto.PaymentLink,
                PaymentExpiration = dto.PaymentExpiration,
                ShippingFee = dto.ShippingFee,
                StatusHistory = System.Text.Json.JsonSerializer.Serialize(statusHistory, new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                }),
                UpdateBy = updatedBy,
                InsertedAt = DateTime.Now,
                Delete = false,
            };

            var created = await _repository.AddAsync(entity);
            var refreshed = await _repository.GetByIdAsync(created.Id) ?? created;
            return Ok(MapToManagementOrderDto(refreshed));
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpPatch("update")]
    public async Task<ActionResult<ManagementOrderResponseDto>> Update([FromBody] OrderUpdateDto dto)
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
                Status = Constant.OrderStatus.GetStatusInt(dto.OrderStatus),
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

            var refreshed = await _repository.GetByIdAsync(updated.Id) ?? updated;
            return Ok(MapToManagementOrderDto(refreshed));
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
    public async Task<ActionResult<ManagementOrderResponseDto>> UpdateStatus(int id, [FromBody] OrderStatusUpdateDto dto)
    {
        try
        {
            var existingOrder = await _repository.GetByIdAsync(id);
            if (existingOrder is null)
                return NotFound("Đơn hàng không tồn tại");

            bool isTransitionToCancel = (dto.OrderStatus == "Đã hủy(KH)" || dto.OrderStatus == "Đã hủy" || dto.PaymentStatus == "Đã hủy")
                && !(existingOrder.OrderStatus == "Đã hủy(KH)" || existingOrder.OrderStatus == "Đã hủy" || existingOrder.PaymentStatus == "Đã hủy");

            var updatedBy = dto.UpdateBy;
            if (string.IsNullOrEmpty(updatedBy))
            {
                updatedBy = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }
            if (string.IsNullOrEmpty(updatedBy))
            {
                updatedBy = "System";
            }

            existingOrder.PaymentStatus = dto.PaymentStatus;
            existingOrder.OrderStatus = dto.OrderStatus;
            existingOrder.Status = Constant.OrderStatus.GetStatusInt(dto.OrderStatus);
            existingOrder.LastUpdate = DateTime.Now;
            existingOrder.UpdateBy = updatedBy;

            var statusHistory = ParseStatusHistory(existingOrder.StatusHistory);
            statusHistory.Add(new StatusHistoryEntry
            {
                Index = statusHistory.Count + 1,
                Status = dto.OrderStatus,
                OrderStatus = dto.OrderStatus,
                PaymentStatus = dto.PaymentStatus,
                DateTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy"),
                UpdatedBy = updatedBy,
                Reasons = dto.Reasons
            });
            
            existingOrder.StatusHistory = System.Text.Json.JsonSerializer.Serialize(statusHistory, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            var updated = await _repository.UpdateAsync(existingOrder);
            if (updated is null)
                return BadRequest("Không thể cập nhật trạng thái đơn hàng");

            if (isTransitionToCancel)
            {
                var pointHistoryRepository = new PointHistoryRepository();
                await pointHistoryRepository.RefundPointsForOrderAsync(existingOrder.Id);
                
                if (dto.RefundStock == true)
                {
                    if (existingOrder.OrderItems != null && existingOrder.OrderItems.Any())
                    {
                        foreach (var orderItem in existingOrder.OrderItems.Where(oi => !oi.Delete))
                        {
                            await _productVariantRepository.IncreaseStockAsync(orderItem.ProductVariantId, orderItem.Quantity);
                        }
                    }
                }

                if (existingOrder.VoucherId != null)
                {
                    var voucher = await _voucherRepository.GetByIdAsync(existingOrder.VoucherId.Value);
                    if (voucher != null && voucher.UsedCount > 0)
                    {
                        voucher.UsedCount--;
                        await _voucherRepository.UpdateAsync(voucher);
                    }
                }
            }

            if (dto.OrderStatus == Constant.OrderStatus.Done)
            {
               
                await ProcessRewardPointsAsync(existingOrder);             

                if (existingOrder.Customer != null && 
                    !string.IsNullOrWhiteSpace(existingOrder.Customer.Email) && 
                    existingOrder.Customer.Email != "guest@example.com")
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            var emailAddress = existingOrder.Customer.Email;
                            var receiverName = existingOrder.Customer.FullName;
                            var subject = "[PickleChic] Đơn hàng #" + existingOrder.OrderCode + " đã hoàn thành thành công!";
                            
                            decimal totalPrice = existingOrder.OrderItems?.Sum(oi => oi.Subtotal) ?? 0;
                            decimal discountAmount = 0;
                            if (existingOrder.Voucher != null)
                            {
                                var voucher = existingOrder.Voucher;
                                if (voucher.DiscountType.StartsWith("Percent", StringComparison.OrdinalIgnoreCase))
                                {
                                    discountAmount = totalPrice * (voucher.DiscountValue / 100);
                                    if (voucher.MaxDiscountAmount.HasValue && discountAmount > voucher.MaxDiscountAmount.Value)
                                    {
                                        discountAmount = voucher.MaxDiscountAmount.Value;
                                    }
                                }
                                else if (voucher.DiscountType.StartsWith("Fixed", StringComparison.OrdinalIgnoreCase))
                                {
                                    discountAmount = voucher.DiscountValue;
                                }
                                discountAmount = Math.Min(discountAmount, totalPrice);
                            }
                            var pointsHistoryEntry = existingOrder.PointHistories?
                                .FirstOrDefault(ph => ph.Points < 0 && ph.TransactionType == "Dùng điểm");
                            int pointsUsed = pointsHistoryEntry != null ? Math.Abs(pointsHistoryEntry.Points) : 0;
                            decimal pointsDiscount = pointsUsed;

                            decimal finalPrice = Math.Max(0, totalPrice - discountAmount - pointsDiscount + existingOrder.ShippingFee);

                            string fullAddress = "";
                            if (existingOrder.Address != null)
                            {
                                var addr = existingOrder.Address;
                                var parts = new List<string>();
                                if (!string.IsNullOrWhiteSpace(addr.DetailInfo)) parts.Add(addr.DetailInfo);
                                if (addr.Ward != null)
                                {
                                    parts.Add(addr.Ward.Name);
                                    if (addr.Ward.District != null)
                                    {
                                        parts.Add(addr.Ward.District.Name);
                                        if (addr.Ward.District.Province != null)
                                        {
                                            parts.Add(addr.Ward.District.Province.Name);
                                        }
                                    }
                                }
                                fullAddress = string.Join(", ", parts);
                            }

                            var itemsTableRows = new System.Text.StringBuilder();
                            if (existingOrder.OrderItems != null)
                            {
                                foreach (var item in existingOrder.OrderItems)
                                {
                                    var productName = item.ProductVariant?.Product?.ProductName ?? "Sản phẩm";
                                    var variantName = item.ProductVariant?.VariantName;
                                    var displayName = string.IsNullOrEmpty(variantName) ? productName : $"{productName} ({variantName})";
                                    var unitPriceStr = item.UnitPrice.ToString("#,##0") + " ₫";
                                    var subtotalStr = item.Subtotal.ToString("#,##0") + " ₫";

                                    itemsTableRows.Append($@"
                                        <tr style=""border-bottom: 1px solid #f1f5f9;"">
                                            <td style=""padding: 12px 0; text-align: left; font-size: 14px; color: #334155;"">
                                                <div style=""font-weight: 600;"">{displayName}</div>
                                            </td>
                                            <td style=""padding: 12px 0; text-align: center; font-size: 14px; color: #64748b;"">
                                                {item.Quantity}
                                            </td>
                                            <td style=""padding: 12px 0; text-align: right; font-size: 14px; color: #64748b;"">
                                                {unitPriceStr}
                                            </td>
                                            <td style=""padding: 12px 0; text-align: right; font-size: 14px; font-weight: bold; color: #0f172a;"">
                                                {subtotalStr}
                                            </td>
                                        </tr>");
                                }
                            }

                            string voucherDiscountRow = "";
                            if (discountAmount > 0)
                            {
                                voucherDiscountRow = $@"
                                    <tr>
                                        <td style=""padding: 6px 0; font-size: 14px; color: #64748b;"">Giảm giá voucher:</td>
                                        <td style=""padding: 6px 0; font-size: 14px; color: #ef4444; text-align: right;"">-{discountAmount.ToString("#,##0")} ₫</td>
                                    </tr>";
                            }

                            string pointDiscountRow = "";
                            if (pointsDiscount > 0)
                            {
                                pointDiscountRow = $@"
                                    <tr>
                                        <td style=""padding: 6px 0; font-size: 14px; color: #64748b;"">Giảm giá điểm tích lũy:</td>
                                        <td style=""padding: 6px 0; font-size: 14px; color: #ef4444; text-align: right;"">-{pointsDiscount.ToString("#,##0")} ₫</td>
                                    </tr>";
                            }

                            var bodyHTML = $@"
                            <!DOCTYPE html>
                            <html lang=""vi"">
                            <head>
                                <meta charset=""UTF-8"">
                                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                <title>Đơn hàng hoàn thành thành công</title>
                            </head>
                            <body style=""margin: 0; padding: 0; font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; background-color: #f6f9fc; -webkit-font-smoothing: antialiased;"">
                                <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed; background-color: #f6f9fc; padding: 40px 0;"">
                                    <tr>
                                        <td align=""center"">
                                            <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""max-width: 600px; background-color: #ffffff; border-radius: 16px; overflow: hidden; box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);"">
                                                <tr>
                                                    <td align=""center"" style=""background: linear-gradient(135deg, #A05AFF 0%, #FE9496 100%); padding: 40px 20px; color: #ffffff;"">
                                                        <h1 style=""margin: 0; font-size: 28px; font-weight: 800; letter-spacing: 2px; text-transform: uppercase;"">PickleChic</h1>
                                                        <p style=""margin: 10px 0 0 0; font-size: 16px; opacity: 0.9; font-weight: 300;"">ĐƠN HÀNG HOÀN THÀNH THÀNH CÔNG</p>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style=""padding: 40px 30px;"">
                                                        <p style=""margin: 0 0 20px 0; font-size: 16px; line-height: 1.6; color: #333333;"">
                                                            Xin chào <strong>{receiverName}</strong>,
                                                        </p>
                                                        <p style=""margin: 0 0 30px 0; font-size: 16px; line-height: 1.6; color: #555555;"">
                                                            Cảm ơn bạn đã mua sắm tại <strong>PickleChic</strong>. Chúng tôi rất vui mừng thông báo đơn hàng của bạn đã được hoàn thành và giao hàng thành công!
                                                        </p>
                                                        
                                                        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: #f8fafc; border-radius: 12px; padding: 20px; margin-bottom: 30px;"">
                                                            <tr>
                                                                <td style=""padding: 4px 0; font-size: 14px; color: #64748b;"">Mã đơn hàng:</td>
                                                                <td style=""padding: 4px 0; font-size: 14px; font-weight: bold; color: #0f172a; text-align: right;"">#{existingOrder.OrderCode}</td>
                                                            </tr>
                                                            <tr>
                                                                <td style=""padding: 4px 0; font-size: 14px; color: #64748b;"">Ngày đặt hàng:</td>
                                                                <td style=""padding: 4px 0; font-size: 14px; color: #0f172a; text-align: right;"">{existingOrder.OrderDate:dd/MM/yyyy HH:mm}</td>
                                                            </tr>
                                                            <tr>
                                                                <td style=""padding: 4px 0; font-size: 14px; color: #64748b;"">Phương thức thanh toán:</td>
                                                                <td style=""padding: 4px 0; font-size: 14px; color: #0f172a; text-align: right;"">{(existingOrder.PaymentMethod?.Name ?? "Thẻ/Ví điện tử/COD")}</td>
                                                            </tr>
                                                        </table>

                                                        <h3 style=""margin: 0 0 15px 0; font-size: 16px; font-weight: 700; color: #0f172a; border-bottom: 2px solid #f1f5f9; padding-bottom: 8px;"">Chi tiết sản phẩm</h3>
                                                        
                                                        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""margin-bottom: 30px; border-collapse: collapse;"">
                                                            <thead>
                                                                <tr style=""border-bottom: 2px solid #e2e8f0;"">
                                                                    <th style=""padding: 10px 0; text-align: left; font-size: 13px; font-weight: 600; color: #64748b; text-transform: uppercase;"">Sản phẩm</th>
                                                                    <th style=""padding: 10px 0; text-align: center; font-size: 13px; font-weight: 600; color: #64748b; text-transform: uppercase; width: 60px;"">SL</th>
                                                                    <th style=""padding: 10px 0; text-align: right; font-size: 13px; font-weight: 600; color: #64748b; text-transform: uppercase; width: 100px;"">Đơn giá</th>
                                                                    <th style=""padding: 10px 0; text-align: right; font-size: 13px; font-weight: 600; color: #64748b; text-transform: uppercase; width: 100px;"">Thành tiền</th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                {itemsTableRows}
                                                            </tbody>
                                                        </table>

                                                        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border-top: 1px solid #e2e8f0; padding-top: 15px; margin-bottom: 35px;"">
                                                            <tr>
                                                                <td style=""padding: 6px 0; font-size: 14px; color: #64748b;"">Tạm tính:</td>
                                                                <td style=""padding: 6px 0; font-size: 14px; color: #0f172a; text-align: right;"">{totalPrice.ToString("#,##0")} ₫</td>
                                                            </tr>
                                                            <tr>
                                                                <td style=""padding: 6px 0; font-size: 14px; color: #64748b;"">Phí vận chuyển:</td>
                                                                <td style=""padding: 6px 0; font-size: 14px; color: #0f172a; text-align: right;"">{existingOrder.ShippingFee.ToString("#,##0")} ₫</td>
                                                            </tr>
                                                            {voucherDiscountRow}
                                                            {pointDiscountRow}
                                                            <tr>
                                                                <td style=""padding: 12px 0 0 0; font-size: 16px; font-weight: bold; color: #0f172a; border-top: 1px dashed #e2e8f0; margin-top: 10px;"">Tổng tiền thanh toán:</td>
                                                                <td style=""padding: 12px 0 0 0; font-size: 20px; font-weight: 800; color: #A05AFF; text-align: right; border-top: 1px dashed #e2e8f0; margin-top: 10px;"">{finalPrice.ToString("#,##0")} ₫</td>
                                                            </tr>
                                                        </table>

                                                        <h3 style=""margin: 0 0 15px 0; font-size: 16px; font-weight: 700; color: #0f172a; border-bottom: 2px solid #f1f5f9; padding-bottom: 8px;"">Thông tin giao nhận</h3>
                                                        <table border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""background-color: #f8fafc; border-radius: 12px; padding: 20px; font-size: 14px; line-height: 1.6; color: #334155;"">
                                                            <tr>
                                                                <td style=""padding-bottom: 8px;"">
                                                                    <strong>Người nhận:</strong> {(existingOrder.Address?.FullName ?? receiverName)}
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td style=""padding-bottom: 8px;"">
                                                                    <strong>Số điện thoại:</strong> {(existingOrder.Address?.PhoneNumber ?? "")}
                                                                </td>
                                                            </tr>
                                                            <tr>
                                                                <td>
                                                                    <strong>Địa chỉ giao hàng:</strong> {fullAddress}
                                                                </td>
                                                            </tr>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td align=""center"" style=""background-color: #f8fafc; padding: 30px; border-top: 1px solid #f1f5f9; text-align: center;"">
                                                        <p style=""margin: 0 0 10px 0; font-size: 13px; color: #64748b; line-height: 1.5;"">
                                                            Nếu bạn có bất kỳ câu hỏi nào về đơn hàng, vui lòng liên hệ với chúng tôi qua email <a href=""mailto:picklechic@proton.me"" style=""color: #A05AFF; text-decoration: none;"">picklechic@proton.me</a>.
                                                        </p>
                                                        <p style=""margin: 0; font-size: 12px; color: #94a3b8;"">
                                                            &copy; 2026 PickleChic. All rights reserved.
                                                        </p>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </body>
                            </html>";

                            var plainBody = "Xin chào " + receiverName + ", Đơn hàng #" + existingOrder.OrderCode + " của bạn đã hoàn thành thành công! Tổng số tiền thanh toán: " + finalPrice.ToString("#,##0") + " ₫. Trân trọng, Đội ngũ PickleChic.";
                            
                            var utilityFunc = new UtilityFunc();
                            await utilityFunc.SendEmailToAddress(emailAddress, receiverName, subject, plainBody, bodyHTML);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Lỗi gửi email đơn hàng hoàn thành: " + ex.Message);
                        }
                    });
                }
            }

            var refreshed = await _repository.GetByIdAsync(id) ?? updated;
            return Ok(MapToManagementOrderDto(refreshed));
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    private static ManagementOrderResponseDto MapToManagementOrderDto(Order order)
    {
        return new ManagementOrderResponseDto
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            OrderCode = order.OrderCode,
            AddressId = order.AddressId,
            OrderDate = order.OrderDate,
            PaymentMethodId = order.PaymentMethodId,
            VoucherId = order.VoucherId,
            PaymentStatus = order.PaymentStatus,
            OrderStatus = order.OrderStatus,
            Status = order.Status,
            Notes = order.Notes,
            LastUpdate = order.LastUpdate,
            CustomerType = order.CustomerType,
            IsOrderPOS = order.IsOrderPOS,
            BOPIS = order.BOPIS,
            PaymentLink = order.PaymentLink,
            PaymentExpiration = order.PaymentExpiration,
            ShippingFee = order.ShippingFee,
            StatusHistory = order.StatusHistory,
            UpdateBy = order.UpdateBy,
            InsertedAt = order.InsertedAt,
            Customer = order.Customer is null
                ? null
                : new ManagementOrderCustomerDto
                {
                    Id = order.Customer.Id,
                    FullName = order.Customer.FullName,
                    Email = order.Customer.Email,
                    PhoneNumber = order.Customer.PhoneNumber
                },
            Address = order.Address is null
                ? null
                : new ManagementOrderAddressDto
                {
                    Id = order.Address.Id,
                    FullName = order.Address.FullName,
                    PhoneNumber = order.Address.PhoneNumber,
                    DetailInfo = order.Address.DetailInfo,
                    Ward = order.Address.Ward is null
                        ? null
                        : new ManagementOrderWardDto
                        {
                            Id = order.Address.Ward.Id,
                            Name = order.Address.Ward.Name,
                            District = order.Address.Ward.District is null
                                ? null
                                : new ManagementOrderDistrictDto
                                {
                                    Id = order.Address.Ward.District.Id,
                                    Name = order.Address.Ward.District.Name,
                                    Province = order.Address.Ward.District.Province is null
                                        ? null
                                        : new ManagementOrderProvinceDto
                                        {
                                            Id = order.Address.Ward.District.Province.Id,
                                            Name = order.Address.Ward.District.Province.Name
                                        }
                                }
                        }
                },
            PaymentMethod = order.PaymentMethod is null
                ? null
                : new ManagementOrderPaymentMethodDto
                {
                    Id = order.PaymentMethod.Id,
                    Name = order.PaymentMethod.Name
                },
            Voucher = order.Voucher is null
                ? null
                : new ManagementOrderVoucherDto
                {
                    Id = order.Voucher.Id,
                    VoucherCode = order.Voucher.VoucherCode,
                    DiscountType = order.Voucher.DiscountType,
                    DiscountValue = order.Voucher.DiscountValue
                },
            OrderItems = order.OrderItems?
                .Where(oi => !oi.Delete)
                .Select(oi => new ManagementOrderItemDto
                {
                    Id = oi.Id,
                    OrderId = oi.OrderId,
                    ProductVariantId = oi.ProductVariantId,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    DiscountAmount = oi.DiscountAmount,
                    Subtotal = oi.Subtotal,
                    ProductVariant = oi.ProductVariant is null
                        ? null
                        : new ManagementOrderProductVariantDto
                        {
                            Id = oi.ProductVariant.Id,
                            SKU = oi.ProductVariant.SKU,
                            VariantName = oi.ProductVariant.VariantName,
                            Product = oi.ProductVariant.Product is null
                                ? null
                                : new ManagementOrderProductDto
                                {
                                    Id = oi.ProductVariant.Product.Id,
                                    ProductName = oi.ProductVariant.Product.ProductName
                                }
                        }
                })
                .ToList()
                ?? new List<ManagementOrderItemDto>()
        };
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

    private async Task ProcessRewardPointsAsync(Order order)
    {
        if (order.CustomerId == -1)
        {
            return;
        }

        bool alreadyRewarded = order.PointHistories?
            .Any(ph => ph.TransactionType == "Cộng điểm") ?? false;
        if (alreadyRewarded)
        {
            return;
        }

        var customer = await _customerRepository.GetByIdAsync(order.CustomerId);
        if (customer == null)
        {
            return;
        }

        decimal totalProductPrice = order.OrderItems?.Sum(oi => oi.Subtotal) ?? 0;
        decimal discountAmount = 0;

        if (order.Voucher != null)
        {
            var voucher = order.Voucher;
            if (voucher.DiscountType.StartsWith("Percent", StringComparison.OrdinalIgnoreCase))
            {
                discountAmount = totalProductPrice * (voucher.DiscountValue / 100);
                if (voucher.MaxDiscountAmount.HasValue && discountAmount > voucher.MaxDiscountAmount.Value)
                {
                    discountAmount = voucher.MaxDiscountAmount.Value;
                }
            }
            else if (voucher.DiscountType.StartsWith("Fixed", StringComparison.OrdinalIgnoreCase))
            {
                discountAmount = voucher.DiscountValue;
            }
            discountAmount = Math.Min(discountAmount, totalProductPrice);
        }

        decimal finalPaidAmount = Math.Max(0, totalProductPrice - discountAmount);

        double percentReward = _configuration.GetValue<double?>("PercentReward") ?? _configuration.GetValue<double?>("RewardPercent") ?? 10.0;
        int pointsToAdd = Math.Max(0, (int)Math.Round((double)finalPaidAmount * percentReward / 100.0));

        if (pointsToAdd > 0)
        {
            var pointHistory = new PointHistory
            {
                CustomerId = customer.Id,
                OrderId = order.Id,
                Points = pointsToAdd,
                TransactionType = "Cộng điểm",
                Description = $"Cộng điểm từ đơn hàng {order.OrderCode}",
                CreatedAt = DateTime.Now
            };

            await _pointHistoryRepository.AddAsync(pointHistory);

            customer.TotalPoints += pointsToAdd;

            decimal totalSpent = await _repository.GetTotalSpentInLast6MonthsAsync(customer.Id);

            var ranks = await _rankRepository.GetAllAsync();
            var qualifiedRank = ranks
                .Where(r => totalSpent >= r.SpendAmount)
                .OrderByDescending(r => r.SpendAmount)
                .FirstOrDefault();

            if (qualifiedRank != null && customer.RankId != qualifiedRank.Id)
            {
                customer.RankId = qualifiedRank.Id;
            }

            await _customerRepository.UpdateAsync(customer);
        }
    }
}
