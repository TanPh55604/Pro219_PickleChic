using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PickleChic.API.DTOs;
using PickleChic.DAL.Models;
using PickleChic.DAL.Repositories;
using System.Security.Claims;
using System.Text.Json;
using Net.payOS;
using Net.payOS.Types;
using Hangfire;
using PickleChic.API.Services;

namespace PickleChic.API.Controllers.Public;

[Route("/order")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly OrderRepository _orderRepository;
    private readonly OrderItemRepository _orderItemRepository;
    private readonly ProductVariantRepository _productVariantRepository;
    private readonly VoucherRepository _voucherRepository;
    private readonly AddressRepository _addressRepository;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly PayOS _payOS;
    private readonly CustomerRepository _customerRepository;
    private readonly RankRepository _rankRepository;

    private static readonly JsonSerializerOptions _camelCaseJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public OrderController(
        OrderRepository orderRepository,
        OrderItemRepository orderItemRepository,
        ProductVariantRepository productVariantRepository,
        VoucherRepository voucherRepository,
        AddressRepository addressRepository,
        IBackgroundJobClient backgroundJobClient,
        PayOS payOS,
        CustomerRepository customerRepository,
        RankRepository rankRepository)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _productVariantRepository = productVariantRepository;
        _voucherRepository = voucherRepository;
        _addressRepository = addressRepository;
        _backgroundJobClient = backgroundJobClient;
        _payOS = payOS;
        _customerRepository = customerRepository;
        _rankRepository = rankRepository;
    }   

    [HttpGet("get-by-id/{id}")]
    public async Task<ActionResult<Order>> GetById(int id)
    {
        try
        {
            var result = await _orderRepository.GetByIdAsync(id);
            if (result is null)
                return NotFound();

            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, "Db Error");
        }
    }

    [HttpPost("Checkout")]
    public async Task<ActionResult<CheckoutDTO>> GetCheckoutUrl(
        [FromBody] CheckoutParamsDTO checkoutParam, 
        [FromQuery] decimal discountAmount = 0, 
        [FromQuery] decimal shippingFee = 0, 
        [FromQuery] int? PaymentMethodTypeId = 2, 
        [FromQuery] int? voucherId = null, 
        [FromQuery] string note = "", 
        [FromQuery] int addressId = -99)
    {
        if (checkoutParam == null || checkoutParam.ListItemCheckout == null || !checkoutParam.ListItemCheckout.Any())
        {
            return BadRequest("Dữ liệu checkout không hợp lệ");
        }

        var variantDict = new Dictionary<int, ProductVariant>();
        foreach (var item in checkoutParam.ListItemCheckout)
        {
            if (item.ProductVariantId <= 0 || item.Quantity <= 0)
            {
                return BadRequest("Sản phẩm bạn đã chọn đã hết");
            }

            var variant = await _productVariantRepository.GetVariantWithDetailsByIdAsync(item.ProductVariantId);
            if (variant == null || variant.Status == -1)
            {
                return BadRequest("Sản phẩm không còn hoạt động hoặc đã bị xóa");
            }

            if (variant.StockQuantity < item.Quantity)
            {
                return BadRequest("Số lượng kho không đủ");
            }

            var product = variant.Product;
            if (product == null || product.IsDeleted || product.Status == -1)
            {
                return BadRequest("Sản phẩm không còn hoạt động hoặc đã bị xóa");
            }

            var brand = product.Brand;
            if (brand == null || brand.Delete || brand.Status == -1)
            {
                return BadRequest("Thương hiệu đã ngừng kinh doanh");
            }

            variantDict[item.ProductVariantId] = variant;
        }

        decimal totalPrice = 0;
        var orderItemDetails = new List<(ProductVariant Variant, int Quantity, int? PromotionId, decimal DiscountAmount, decimal Subtotal)>();

        foreach (var product in checkoutParam.ListItemCheckout)
        {
            var variant = variantDict[product.ProductVariantId];
            
            // Find active promotion
            var activePromoDetail = variant.PromotionDetails?
                .FirstOrDefault(pd => pd.Promotion != null 
                                      && pd.Promotion.IsActive 
                                      && DateTime.Now >= pd.Promotion.StartDate 
                                      && DateTime.Now <= pd.Promotion.EndDate);

            int? itemPromotionId = null;
            decimal itemDiscountAmount = 0;

            if (activePromoDetail != null)
            {
                itemPromotionId = activePromoDetail.PromotionId;
                if (activePromoDetail.DiscountType.StartsWith("Percent", StringComparison.OrdinalIgnoreCase))
                {
                    itemDiscountAmount = variant.Price * (activePromoDetail.DiscountValue / 100);
                }
                else if (activePromoDetail.DiscountType.StartsWith("Fixed", StringComparison.OrdinalIgnoreCase))
                {
                    itemDiscountAmount = activePromoDetail.DiscountValue;
                }
                itemDiscountAmount = Math.Min(itemDiscountAmount, variant.Price);
            }

            decimal itemSubtotal = (variant.Price - itemDiscountAmount) * product.Quantity;
            totalPrice += itemSubtotal;

            orderItemDetails.Add((variant, product.Quantity, itemPromotionId, itemDiscountAmount, itemSubtotal));
        }

        List<ItemData> items = new List<ItemData>();
        foreach (var detail in orderItemDetails)
        {
            int unitPriceAfterPromo = (int)(detail.Variant.Price - detail.DiscountAmount);
            ItemData item = new ItemData(detail.Variant.VariantName ?? detail.Variant.Product?.ProductName ?? "Sản phẩm", detail.Quantity, unitPriceAfterPromo);
            items.Add(item);
        }

        Address address = null!;
        var claimVal = User.FindFirst(ClaimTypes.SerialNumber)?.Value;
        int customerId = -1;
        if (!string.IsNullOrEmpty(claimVal) && int.TryParse(claimVal, out var parsedId))
        {
            customerId = parsedId;
        }

        string? phoneNumber = null;
        if (checkoutParam.AddressDTO != null)
        {
            phoneNumber = checkoutParam.AddressDTO.PhoneNumber;
        }
        else if (addressId != -99)
        {
            address = await _addressRepository.GetByIdAsync(addressId);
            phoneNumber = address?.PhoneNumber;
        }

        if (voucherId != null && customerId!=-1)
        {
            var (discount, errorMsg) = await ApplyDiscount(totalPrice, voucherId.Value, customerId, phoneNumber);
            if (errorMsg != null)
            {
                return BadRequest(errorMsg);
            }
            discountAmount = discount;
        }
        else
        {
            discountAmount = 0;
        }

        if (checkoutParam.AddressDTO != null && addressId == -99)
        {
            address = new Address
            {
                CustomerId = customerId, 
                FullName = checkoutParam.AddressDTO.FullName,
                PhoneNumber = checkoutParam.AddressDTO.PhoneNumber,
                WardId = checkoutParam.AddressDTO.WardId,
                DetailInfo = checkoutParam.AddressDTO.DetailInfo,
                IsDefault = checkoutParam.AddressDTO.IsDefault,
                Status = checkoutParam.AddressDTO.Status,
                InsertedAt = DateTime.Now,
                Delete = false
            };
            var resultAddress = await _addressRepository.AddAsync(address);
            if (resultAddress == null)
            {
                return BadRequest("Không thể thêm địa chỉ giao hàng");
            }
            addressId = resultAddress.Id;
        }

        if (addressId == -99)
        {
            return BadRequest("Địa chỉ giao hàng không hợp lệ");
        }

        if (address == null)
        {
            address = await _addressRepository.GetByIdAsync(addressId);
        }

        if (address == null)
        {
            return BadRequest("Địa chỉ giao hàng không tồn tại");
        }

        bool isZeroOrder = (totalPrice - discountAmount + shippingFee) <= 0;
        int ordCode = new Random().Next(1, int.MaxValue);
        Order order = new Order();
        try
        {
            var statusHistory = ParseStatusHistory(order.StatusHistory);
            if (isZeroOrder)
            {
                statusHistory.Add(new StatusHistoryEntry
                {
                    Index = statusHistory.Count + 1,
                    Status = Constant.OrderStatus.StatusConfirm,
                    OrderStatus = Constant.OrderStatus.OrderStatusConfirm,
                    PaymentStatus = Constant.OrderStatus.PaymentCompleted,
                    DateTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy")
                });
            }
            else if (PaymentMethodTypeId == 1)
            {
                statusHistory.Add(new StatusHistoryEntry
                {
                    Index = statusHistory.Count + 1,
                    Status = Constant.OrderStatus.StatusPending,
                    OrderStatus = Constant.OrderStatus.OrderStatusPending,
                    PaymentStatus = Constant.OrderStatus.PaymentPending,
                    DateTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy")
                });
            }
            else
            {
                statusHistory.Add(new StatusHistoryEntry
                {
                    Index = statusHistory.Count + 1,
                    Status = Constant.OrderStatus.StatusWaitingForPayment,
                    OrderStatus = Constant.OrderStatus.OrderStatusWaitingForPayment,
                    PaymentStatus = Constant.OrderStatus.PaymentPending,
                    DateTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy")
                });
            }
            order.StatusHistory = JsonSerializer.Serialize(statusHistory, _camelCaseJsonOptions);

            order.OrderCode = "DH" + ordCode.ToString();
            order.AddressId = addressId;
            order.Notes = note;
            order.ShippingFee = shippingFee;
            order.OrderDate = DateTime.Now;
            if (isZeroOrder)
            {
                order.PaymentStatus = Constant.OrderStatus.PaymentCompleted;
                order.OrderStatus = Constant.OrderStatus.OrderStatusConfirm;
            }
            else
            {
                order.PaymentStatus = Constant.OrderStatus.PaymentPending;
                order.OrderStatus = PaymentMethodTypeId == 2 ? Constant.OrderStatus.OrderStatusWaitingForPayment : Constant.OrderStatus.OrderStatusPending;
            }
            order.VoucherId = voucherId;
            order.InsertedAt = DateTime.Now;
            order.LastUpdate = DateTime.Now;
            order.UpdateBy = "System";
            order.IsOrderPOS = false;
            order.Delete = false;

            order.CustomerId = customerId;
            order.CustomerType = checkoutParam.AddressDTO.CustomerId != -1 ? Constant.CustomerType.RegisteredOrder : Constant.CustomerType.GuestOrder;
            order.PaymentMethodId = PaymentMethodTypeId ?? 2;

            var result = await _orderRepository.AddAsync(order);
            if (result == null)
            {
                return BadRequest("Không thể tạo đơn hàng");
            }

            foreach (var detail in orderItemDetails)
            {
                OrderItem orderItem = new OrderItem
                {
                    OrderId = result.Id,
                    ProductVariantId = detail.Variant.Id,
                    PromotionId = detail.PromotionId,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.Variant.Price,
                    DiscountAmount = detail.DiscountAmount,
                    Subtotal = detail.Subtotal,
                    IsReviewed = false,
                    InsertedAt = DateTime.Now,
                    Delete = false
                };
                var resultItem = await _orderItemRepository.AddAsync(orderItem);
                if (resultItem == null)
                {
                    return BadRequest("Không thể thêm chi tiết đơn hàng");
                }
            }
        }
        catch (Exception)
        {
            return StatusCode(500, Constant.ErrorCode.OtherError);
        }

        if (voucherId != null && discountAmount > 0)
        {
            var voucher = await _voucherRepository.GetByIdAsync((int)voucherId);
            if (voucher != null)
            {
                voucher.UsedCount++;
                var resultVoucher = await _voucherRepository.UpdateAsync(voucher);
                if (resultVoucher == null)
                {
                    return BadRequest(Constant.ErrorCode.DatabaseError);
                }
            }
        }

        CheckoutDTO checkoutDTO = new CheckoutDTO
        {
            OrderCode = order.OrderCode,
            OrderId = order.Id,
            PaymentType = PaymentMethodTypeId
        };

        if (isZeroOrder)
        {
            checkoutDTO.URLPayment = null;

            foreach (var product in checkoutParam.ListItemCheckout)
            {
                var decreased = await _productVariantRepository.DecreaseStockAsync(product.ProductVariantId, product.Quantity);
                if (!decreased)
                {
                    return BadRequest("Số lượng kho không đủ hoặc sản phẩm không tồn tại");
                }
            }
            return Ok(checkoutDTO);
        }

        if (PaymentMethodTypeId == 2)
        {
            DateTimeOffset utcNow = DateTimeOffset.UtcNow;
            DateTimeOffset expirationTime = utcNow.AddMinutes(15);
            long expiredAt = expirationTime.ToUnixTimeSeconds();
            int payOsAmount = (int)(totalPrice - discountAmount + shippingFee);

            string cancelUrl = "http://localhost:5001/order/PaymentCanceled?orderId=" + order.Id;
            string returnUrl = "http://localhost:5001/order/PaymentSuccess?orderId=" + order.Id + "&pos=false";

            PaymentData paymentData = new PaymentData(
                ordCode, 
                payOsAmount, 
                "PickleChic Thanh toan DH", 
                items, 
                cancelUrl, 
                returnUrl, 
                expiredAt: expiredAt
            );

            CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);

            if (createPayment.status == "PENDING")
            {
                checkoutDTO.URLPayment = createPayment.checkoutUrl;

                foreach (var product in checkoutParam.ListItemCheckout)
                {
                    var decreased = await _productVariantRepository.DecreaseStockAsync(product.ProductVariantId, product.Quantity);
                    if (!decreased)
                    {
                        return BadRequest("Số lượng kho không đủ hoặc sản phẩm không tồn tại");
                    }
                }

                order.PaymentExpiration = DateTime.Now.AddMinutes(15);
                order.PaymentLink = createPayment.checkoutUrl;
                await _orderRepository.UpdateAsync(order);

                _backgroundJobClient.Schedule<OrderManagerService>(
                    x => x.CancelExpiredOrderAsync(order.Id),
                    TimeSpan.FromMinutes(15)
                );

                return Ok(checkoutDTO);
            }
            else
            {
                return BadRequest("Không thể tạo liên kết thanh toán");
            }
        }
        else if (PaymentMethodTypeId == 1)
        {
            checkoutDTO.URLPayment = null;

            foreach (var product in checkoutParam.ListItemCheckout)
            {
                var decreased = await _productVariantRepository.DecreaseStockAsync(product.ProductVariantId, product.Quantity);
                if (!decreased)
                {
                    return BadRequest("Số lượng kho không đủ hoặc sản phẩm không tồn tại");
                }
            }
            return Ok(checkoutDTO);
        }

        return BadRequest("Phương thức thanh toán không hợp lệ");
    }

    [HttpGet("PaymentSuccess")]
    public async Task<ActionResult<Order>> PaymentSuccess([FromQuery] int orderId, [FromQuery] bool pos, [FromQuery] string? errorMessage = null)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại");
            }

            var statusHistory = ParseStatusHistory(order.StatusHistory);
            if (pos)
            {
                statusHistory.Add(new StatusHistoryEntry
                {
                    Index = statusHistory.Count + 1,
                    Status = Constant.OrderStatus.StatusDone,
                    OrderStatus = Constant.OrderStatus.OrderStatusDone,
                    PaymentStatus = Constant.OrderStatus.PaymentCompleted,
                    DateTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy")
                });

                order.PaymentStatus = Constant.OrderStatus.PaymentCompleted;
                order.OrderStatus = Constant.OrderStatus.OrderStatusDone;
            }
            else
            {
                statusHistory.Add(new StatusHistoryEntry
                {
                    Index = statusHistory.Count + 1,
                    Status = Constant.OrderStatus.StatusConfirm,
                    OrderStatus = Constant.OrderStatus.OrderStatusConfirm,
                    PaymentStatus = Constant.OrderStatus.PaymentCompleted,
                    DateTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy")
                });

                order.PaymentStatus = Constant.OrderStatus.PaymentCompleted;
                order.OrderStatus = Constant.OrderStatus.OrderStatusConfirm;
            }

            order.StatusHistory = JsonSerializer.Serialize(statusHistory, _camelCaseJsonOptions);
            order.LastUpdate = DateTime.Now;
            order.UpdateBy = "System";

            var result = await _orderRepository.UpdateAsync(order);
            if (result == null)
            {
                return BadRequest("Không thể cập nhật đơn hàng");
            }
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, Constant.ErrorCode.OtherError);
        }
    }

    [HttpGet("PaymentCanceled")]
    public async Task<ActionResult<Order>> PaymentCanceled([FromQuery] int orderId, [FromQuery] string? errorMessage = null)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại");
            }

            if (order.OrderStatus == Constant.OrderStatus.StatusCanceledByUser)
            {
                return Ok(order);
            }

            var statusHistory = ParseStatusHistory(order.StatusHistory);
            statusHistory.Add(new StatusHistoryEntry
            {
                Index = statusHistory.Count + 1,
                Status = Constant.OrderStatus.StatusCanceledByUser,
                OrderStatus = Constant.OrderStatus.OrderStatusCanceledByUser,
                PaymentStatus = Constant.OrderStatus.PaymentCancelled,
                DateTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy")
            });

            order.StatusHistory = JsonSerializer.Serialize(statusHistory, _camelCaseJsonOptions);
            order.PaymentStatus = Constant.OrderStatus.PaymentCancelled;
            order.OrderStatus = Constant.OrderStatus.OrderStatusCanceledByUser;
            order.LastUpdate = DateTime.Now;
            order.UpdateBy = "System";

            var result = await _orderRepository.UpdateAsync(order);
            if (result == null)
            {
                return BadRequest("Không thể cập nhật đơn hàng");
            }

            if (order.OrderItems != null && order.OrderItems.Any())
            {
                foreach (var orderItem in order.OrderItems)
                {
                    await _productVariantRepository.IncreaseStockAsync(orderItem.ProductVariantId, orderItem.Quantity);
                }
            }

            if (order.VoucherId != null)
            {
                var voucher = await _voucherRepository.GetByIdAsync(order.VoucherId.Value);
                if (voucher != null && voucher.UsedCount > 0)
                {
                    voucher.UsedCount--;
                    await _voucherRepository.UpdateAsync(voucher);
                }
            }

            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, Constant.ErrorCode.OtherError);
        }
    }

    private async Task<(decimal discountAmount, string? errorMessage)> ApplyDiscount(decimal totalPrice, int voucherId, int customerId, string? phoneNumber)
    {
        var voucher = await _voucherRepository.GetByIdAsync(voucherId);
        if (voucher == null)
        {
            return (0, "Voucher không tồn tại");
        }

        if (!voucher.IsActive)
        {
            return (0, "Voucher hiện không hoạt động");
        }

        var now = DateTime.Now;
        if (now < voucher.StartDate)
        {
            return (0, "Voucher chưa đến thời gian bắt đầu sử dụng");
        }
        if (now > voucher.EndDate)
        {
            return (0, "Voucher đã hết hạn sử dụng");
        }

        if (totalPrice < voucher.MinOrderValue)
        {
            return (0, $"Giá trị đơn hàng chưa đạt mức tối thiểu {voucher.MinOrderValue:N0}đ để áp dụng voucher");
        }

        if (voucher.UsageLimit > 0 && voucher.UsedCount >= voucher.UsageLimit)
        {
            return (0, "Voucher đã hết lượt sử dụng");
        }

        if (voucher.CustomerUsageLimit > 0)
        {
            int customerUsageCount = await _orderRepository.GetVoucherUsageCountAsync(customerId, voucherId, phoneNumber);
            if (customerUsageCount >= voucher.CustomerUsageLimit)
            {
                return (0, "Bạn đã sử dụng hết số lần cho phép đối với voucher này");
            }
        }

        if (voucher.MinimumPointRank != null)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                return (0, "Không thể xác minh thông tin khách hàng");
            }
            var rank = await _rankRepository.GetByIdAsync(customer.RankId);
            if (rank == null || rank.MinPoints < voucher.MinimumPointRank.Value)
            {
                return (0, "Hạng thành viên của bạn chưa đủ điều kiện để sử dụng voucher này");
            }
        }

        decimal discount = 0;
        if (voucher.DiscountType.StartsWith("Percent", StringComparison.OrdinalIgnoreCase))
        {
            discount = totalPrice * (voucher.DiscountValue / 100);
            if (voucher.MaxDiscountAmount.HasValue && discount > voucher.MaxDiscountAmount.Value)
            {
                discount = voucher.MaxDiscountAmount.Value;
            }
        }
        else if (voucher.DiscountType.StartsWith("Fixed", StringComparison.OrdinalIgnoreCase))
        {
            discount = voucher.DiscountValue;
        }
        else
        {
            return (0, "Loại giảm giá của voucher không hợp lệ");
        }

        discount = Math.Min(discount, totalPrice);
        return (discount, null);
    }

    [HttpGet("user/detail/{orderId}")]
    [Authorize]
    public async Task<ActionResult<UserOrderDetailDto>> GetUserOrderDetail(int orderId)
    {
        try
        {
            var claimVal = User.FindFirst(ClaimTypes.SerialNumber)?.Value;
            if (string.IsNullOrEmpty(claimVal) || !int.TryParse(claimVal, out var customerId))
            {
                return Unauthorized("Không thể xác định thông tin người dùng từ token.");
            }

            var order = await _orderRepository.GetOrderDetailForUserAsync(orderId, customerId);
            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại hoặc bạn không có quyền truy cập.");
            }

            var dto = MapToUserOrderDetailDto(order);
            return Ok(dto);
        }
        catch (Exception)
        {
            return StatusCode(500, Constant.ErrorCode.OtherError);
        }
    }

    [HttpGet("user/list")]
    [Authorize]
    public async Task<ActionResult<List<UserOrderDetailDto>>> GetUserOrders()
    {
        try
        {
            var claimVal = User.FindFirst(ClaimTypes.SerialNumber)?.Value;
            if (string.IsNullOrEmpty(claimVal) || !int.TryParse(claimVal, out var customerId))
            {
                return Unauthorized("Không thể xác định thông tin người dùng từ token.");
            }

            var orders = await _orderRepository.GetOrdersForUserAsync(customerId);
            var dtos = orders.Select(MapToUserOrderDetailDto).ToList();
            return Ok(dtos);
        }
        catch (Exception)
        {
            return StatusCode(500, Constant.ErrorCode.OtherError);
        }
    }

    private UserOrderDetailDto MapToUserOrderDetailDto(Order order)
    {
        decimal totalPrice = order.OrderItems?.Sum(oi => oi.Subtotal) ?? 0;
        decimal discountAmount = 0;

        if (order.Voucher != null)
        {
            var voucher = order.Voucher;
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

        string fullAddress = "";
        if (order.Address != null)
        {
            var addr = order.Address;
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

        return new UserOrderDetailDto
        {
            Id = order.Id,
            OrderCode = order.OrderCode,
            OrderDate = order.OrderDate,
            ShippingFee = order.ShippingFee,
            Notes = order.Notes ?? "",
            PurchaseChannel = order.IsOrderPOS ? "Mua tại cửa hàng (POS)" : "Mua trực tuyến",
            PaymentStatus = order.PaymentStatus switch
            {
                "Pending" => "Chờ thanh toán",
                "Completed" => "Đã thanh toán",
                "Cancelled" => "Đã hủy",
                _ => order.PaymentStatus
            },
            OrderStatus = order.OrderStatus switch
            {
                "Pending" => "Chờ xác nhận",
                "WaitingForPayment" => "Chờ thanh toán",
                "Confirmed" => "Đã xác nhận",
                "Done" => "Hoàn thành",
                "Cancelled" => "Đã hủy",
                "Expired" => "Hết hạn thanh toán",
                _ => order.OrderStatus
            },
            PaymentLink = order.PaymentLink,
            ReceiverName = order.Address?.FullName ?? "",
            ReceiverPhone = order.Address?.PhoneNumber ?? "",
            FullAddress = fullAddress,
            TotalPrice = totalPrice,
            DiscountAmount = discountAmount,
            FinalPrice = Math.Max(0, totalPrice - discountAmount + order.ShippingFee),
            OrderItems = order.OrderItems?.Select(oi => new UserOrderItemDetailDto
            {
                ProductVariantId = oi.ProductVariantId,
                ProductName = oi.ProductVariant?.Product?.ProductName ?? "Sản phẩm",
                VariantName = oi.ProductVariant?.VariantName ?? "",
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice,
                DiscountAmount = oi.DiscountAmount,
                Subtotal = oi.Subtotal,
                Attributes = oi.ProductVariant?.ProductVariantAttributes?
                    .Select(pva => $"{pva.AttributeValue?.ProductAttribute?.AttributeName}: {pva.AttributeValue?.Value}")
                    .ToList() ?? new List<string>()
            }).ToList() ?? new List<UserOrderItemDetailDto>()
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
            return JsonSerializer.Deserialize<List<StatusHistoryEntry>>(json, _camelCaseJsonOptions) 
                   ?? new List<StatusHistoryEntry>();
        }
        catch
        {
            return new List<StatusHistoryEntry>();
        }
    }
}
