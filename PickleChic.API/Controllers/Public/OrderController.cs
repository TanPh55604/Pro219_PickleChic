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
using Microsoft.Extensions.Configuration;
using System.Text;

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
    private readonly IConfiguration _configuration;
    private readonly PointHistoryRepository _pointHistoryRepository;
    private readonly WardRepository _wardRepository;

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
        RankRepository rankRepository,
        IConfiguration configuration,
        PointHistoryRepository pointHistoryRepository,
        WardRepository wardRepository)
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
        _configuration = configuration;
        _pointHistoryRepository = pointHistoryRepository;
        _wardRepository = wardRepository;
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

    [HttpPost("CalculateTotal")]
    public async Task<ActionResult<OrderCalculationResultDto>> CalculateTotal([FromBody] OrderCalculationRequestDto request)
    {
        if (request == null || request.Items == null || !request.Items.Any())
        {
            return BadRequest("Dữ liệu tính tiền không hợp lệ");
        }

        var variantDict = new Dictionary<int, ProductVariant>();
        foreach (var item in request.Items)
        {
            if (item.ProductVariantId <= 0 || item.Quantity <= 0)
            {
                return BadRequest("Sản phẩm bạn đã chọn đã hết hoặc số lượng không hợp lệ");
            }

            var variant = await _productVariantRepository.GetVariantWithDetailsByIdAsync(item.ProductVariantId);
            if (variant == null || variant.Status == -1)
            {
                return BadRequest("Sản phẩm không còn hoạt động hoặc đã bị xóa");
            }

            if (variant.StockQuantity < item.Quantity)
            {
                return BadRequest($"Số lượng kho của biến thể '{variant.VariantName}' không đủ");
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

        decimal totalAmount = 0;
        var itemResults = new List<OrderCalculationItemResultDto>();

        foreach (var item in request.Items)
        {
            var variant = variantDict[item.ProductVariantId];
            
            var activePromoDetail = variant.PromotionDetails?
                .FirstOrDefault(pd => pd.Promotion != null 
                                      && pd.Promotion.IsActive 
                                      && DateTime.Now >= pd.Promotion.StartDate 
                                      && DateTime.Now <= pd.Promotion.EndDate);

            decimal itemDiscountAmount = 0;

            if (activePromoDetail != null)
            {
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

            decimal priceToPay = variant.Price - itemDiscountAmount;
            totalAmount += priceToPay * item.Quantity;

            var attributeNames = string.Empty;
            var attributeValues = string.Empty;

            if (variant.ProductVariantAttributes != null && variant.ProductVariantAttributes.Any())
            {
                attributeNames = string.Join(", ", variant.ProductVariantAttributes
                    .Select(pva => pva.AttributeValue?.ProductAttribute?.AttributeName)
                    .Where(name => !string.IsNullOrEmpty(name)));

                attributeValues = string.Join(", ", variant.ProductVariantAttributes
                    .Select(pva => pva.AttributeValue?.Value)
                    .Where(val => !string.IsNullOrEmpty(val)));
            }

            itemResults.Add(new OrderCalculationItemResultDto
            {
                ProductVariantId = item.ProductVariantId,
                ProductName = variant.Product?.ProductName ?? "Sản phẩm",
                VariantName = variant.VariantName ?? variant.Product?.ProductName ?? "Biến thể",
                AttributeName = attributeNames,
                AttributeValue = attributeValues,
                Quantity = item.Quantity,
                ListedPrice = variant.Price,
                DiscountAmount = itemDiscountAmount,
                PriceToPay = priceToPay
            });
        }
        var claimVal = User.FindFirst(ClaimTypes.SerialNumber)?.Value;
        int customerId = -1;
        if (!string.IsNullOrEmpty(claimVal) && int.TryParse(claimVal, out var parsedId))
        {
            customerId = parsedId;
        }

        decimal shippingFee = 0;
        string? phoneNumber = null;
        bool isBopis = request.Bopis == true;

        if (isBopis)
        {
            shippingFee = 0;
        }
        else if (request.AddressId.HasValue)
        {
            var address = await _addressRepository.GetByIdAsync(request.AddressId.Value);
            if (address != null)
            {
                phoneNumber = address.PhoneNumber;

                var toDistrictCode = address.Ward?.District?.Code;
                var toWardCode = address.Ward?.Code;

                if (!string.IsNullOrEmpty(toDistrictCode) && !string.IsNullOrEmpty(toWardCode))
                {
                    var feeItems = itemResults.Select(i => new FeeItemDTO
                    {
                        Name = i.ProductName,
                        Quantity = i.Quantity,
                        Length = 30,
                        Width = 40,
                        Height = 5,
                        Weight = 400
                    }).ToList();

                    shippingFee = await CalculateShippingFeeAsync(toDistrictCode, toWardCode, feeItems);
                }
            }
            else
            {
                return BadRequest("Địa chỉ nhận hàng không hợp lệ");
            }
        }
        else
        {
            return BadRequest("Địa chỉ nhận hàng không hợp lệ");
        }
            

        decimal discountPrice = 0;
        int? appliedVoucherId = null;
        if (!string.IsNullOrEmpty(request.DiscountCode))
        {
            var voucher = await _voucherRepository.GetByCodeAsync(request.DiscountCode);
            if (voucher == null)
            {
                return BadRequest("Mã giảm giá không tồn tại hoặc đã bị xóa");
            }

            var (calculatedDiscount, errorMessage) = await ApplyDiscount(totalAmount, voucher.Id, customerId, phoneNumber);
            if (errorMessage != null)
            {
                return BadRequest(errorMessage);
            }
            discountPrice = calculatedDiscount;
            appliedVoucherId = voucher.Id;
        }

        decimal tempFinalAmount = totalAmount - discountPrice + shippingFee;
        int pointsDeducted = 0;
        decimal pointsDiscountPrice = 0;

        if (request.UsePoints == true && customerId != -1)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer != null && customer.TotalPoints > 0)
            {
                pointsDeducted = (int)Math.Min((decimal)customer.TotalPoints, Math.Max(0, tempFinalAmount));
                pointsDiscountPrice = pointsDeducted;
            }
        }

        decimal finalAmount = Math.Max(0, tempFinalAmount - pointsDiscountPrice);

        var resultDto = new OrderCalculationResultDto
        {
            TotalAmount = totalAmount,
            DiscountPrice = discountPrice,
            ShippingFee = shippingFee,
            FinalAmount = finalAmount,
            VoucherId = appliedVoucherId,
            Items = itemResults,
            PointsDiscountPrice = pointsDiscountPrice,
            PointsDeducted = pointsDeducted
        };

        return Ok(resultDto);
    }

    [HttpPost("CalculateTotalPOS")]
    public async Task<ActionResult<OrderCalculationResultDto>> CalculateTotalPOS([FromBody] PosOrderCalculationRequestDto request)
    {
        if (request == null || request.Items == null || !request.Items.Any())
        {
            return BadRequest("Dữ liệu tính tiền không hợp lệ");
        }

        var variantDict = new Dictionary<int, ProductVariant>();
        foreach (var item in request.Items)
        {
            if (item.ProductVariantId <= 0 || item.Quantity <= 0)
            {
                return BadRequest("Sản phẩm bạn đã chọn đã hết hoặc số lượng không hợp lệ");
            }

            var variant = await _productVariantRepository.GetVariantWithDetailsByIdAsync(item.ProductVariantId);
            if (variant == null || variant.Status == -1)
            {
                return BadRequest("Sản phẩm không còn hoạt động hoặc đã bị xóa");
            }

            if (variant.StockQuantity < item.Quantity)
            {
                return BadRequest($"Số lượng kho của sản phẩm '{variant.VariantName}' không đủ");
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

        decimal totalAmount = 0;
        var itemResults = new List<OrderCalculationItemResultDto>();

        foreach (var item in request.Items)
        {
            var variant = variantDict[item.ProductVariantId];
            
            var activePromoDetail = variant.PromotionDetails?
                .FirstOrDefault(pd => pd.Promotion != null 
                                      && pd.Promotion.IsActive 
                                      && DateTime.Now >= pd.Promotion.StartDate 
                                      && DateTime.Now <= pd.Promotion.EndDate);

            decimal itemDiscountAmount = 0;

            if (activePromoDetail != null)
            {
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

            decimal priceToPay = variant.Price - itemDiscountAmount;
            totalAmount += priceToPay * item.Quantity;

            var attributeNames = string.Empty;
            var attributeValues = string.Empty;

            if (variant.ProductVariantAttributes != null && variant.ProductVariantAttributes.Any())
            {
                attributeNames = string.Join(", ", variant.ProductVariantAttributes
                    .Select(pva => pva.AttributeValue?.ProductAttribute?.AttributeName)
                    .Where(name => !string.IsNullOrEmpty(name)));

                attributeValues = string.Join(", ", variant.ProductVariantAttributes
                    .Select(pva => pva.AttributeValue?.Value)
                    .Where(val => !string.IsNullOrEmpty(val)));
            }

            itemResults.Add(new OrderCalculationItemResultDto
            {
                ProductVariantId = item.ProductVariantId,
                ProductName = variant.Product?.ProductName ?? "Sản phẩm",
                VariantName = variant.VariantName ?? variant.Product?.ProductName ?? "Biến thể",
                AttributeName = attributeNames,
                AttributeValue = attributeValues,
                Quantity = item.Quantity,
                ListedPrice = variant.Price,
                DiscountAmount = itemDiscountAmount,
                PriceToPay = priceToPay
            });
        }

        int customerId = -1;
        decimal shippingFee = 0;
        string? phoneNumber = null;

        if (request.CustomerId.HasValue && request.CustomerId.Value > 0)
        {
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId.Value);
            if (customer is null || customer.Status <= 0)
            {
                return BadRequest("Khách hàng không hợp lệ");
            }

            customerId = customer.Id;
        }

        if (request.IsShipping && request.AddressId.HasValue && request.AddressId.Value > 0)
        {
            var address = await _addressRepository.GetByIdAsync(request.AddressId.Value);
            if (address != null)
            {
                phoneNumber = address.PhoneNumber;
                if (customerId <= 0)
                {
                    customerId = address.CustomerId;
                }

                var toDistrictCode = address.Ward?.District?.Code;
                var toWardCode = address.Ward?.Code;

                if (!string.IsNullOrEmpty(toDistrictCode) && !string.IsNullOrEmpty(toWardCode))
                {
                    var feeItems = itemResults.Select(i => new FeeItemDTO
                    {
                        Name = i.ProductName,
                        Quantity = i.Quantity,
                        Length = 30,
                        Width = 40,
                        Height = 5,
                        Weight = 400
                    }).ToList();

                    shippingFee = await CalculateShippingFeeAsync(toDistrictCode, toWardCode, feeItems);
                }
            }
            else
            {
                return BadRequest("Địa chỉ nhận hàng không hợp lệ");
            }
        }
        else if (request.IsShipping && request.AddressDTO != null)
        {
            phoneNumber = request.AddressDTO.PhoneNumber;
            if (customerId <= 0 && !string.IsNullOrEmpty(phoneNumber))
            {
                var customer = await _customerRepository.FindUserExistByKeyWord(phoneNumber);
                if (customer != null)
                {
                    customerId = customer.Id;
                }
            }

            if (request.AddressDTO.WardId > 0)
            {
                var ward = await _wardRepository.GetByIdAsync(request.AddressDTO.WardId);
                if (ward == null)
                {
                    return BadRequest("Phường xã giao hàng không hợp lệ");
                }

                var toDistrictCode = ward.District?.Code;
                var toWardCode = ward.Code;

                if (!string.IsNullOrEmpty(toDistrictCode) && !string.IsNullOrEmpty(toWardCode))
                {
                    var feeItems = itemResults.Select(i => new FeeItemDTO
                    {
                        Name = i.ProductName,
                        Quantity = i.Quantity,
                        Length = 30,
                        Width = 40,
                        Height = 5,
                        Weight = 400
                    }).ToList();

                    shippingFee = await CalculateShippingFeeAsync(toDistrictCode, toWardCode, feeItems);
                }
            }
        }

        if (customerId == -1)
        {
            if (!string.IsNullOrEmpty(request.DiscountCode))
            {
                return BadRequest("Chỉ khách hàng thành viên mới được dùng voucher");
            }
            if (request.UsePoints == true)
            {
                return BadRequest("Chỉ khách hàng thành viên mới được dùng điểm tích lũy");
            }
        }

        decimal discountPrice = 0;
        int? appliedVoucherId = null;
        if (!string.IsNullOrEmpty(request.DiscountCode) && customerId != -1)
        {
            var voucher = await _voucherRepository.GetByCodeAsync(request.DiscountCode);
            if (voucher == null)
            {
                return BadRequest("Mã giảm giá không tồn tại hoặc đã bị xóa");
            }

            var (calculatedDiscount, errorMessage) = await ApplyDiscount(totalAmount, voucher.Id, customerId, phoneNumber);
            if (errorMessage != null)
            {
                return BadRequest(errorMessage);
            }
            discountPrice = calculatedDiscount;
            appliedVoucherId = voucher.Id;
        }

        decimal tempFinalAmount = totalAmount - discountPrice + shippingFee;
        int pointsDeducted = 0;
        decimal pointsDiscountPrice = 0;

        if (request.UsePoints == true && customerId != -1)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer != null && customer.TotalPoints > 0)
            {
                pointsDeducted = (int)Math.Min((decimal)customer.TotalPoints, Math.Max(0, tempFinalAmount));
                pointsDiscountPrice = pointsDeducted;
            }
        }

        decimal finalAmount = Math.Max(0, tempFinalAmount - pointsDiscountPrice);

        var resultDto = new OrderCalculationResultDto
        {
            TotalAmount = totalAmount,
            DiscountPrice = discountPrice,
            ShippingFee = shippingFee,
            FinalAmount = finalAmount,
            VoucherId = appliedVoucherId,
            Items = itemResults,
            PointsDiscountPrice = pointsDiscountPrice,
            PointsDeducted = pointsDeducted
        };

        return Ok(resultDto);
    }

    [HttpPost("Checkout")]
    public async Task<ActionResult<CheckoutDTO>> GetCheckoutUrl(
        [FromBody] CheckoutParamsDTO checkoutParam, 
        [FromQuery] decimal discountAmount = 0, 
        [FromQuery] decimal shippingFee = 0, 
        [FromQuery] int? PaymentMethodTypeId = 2, 
        [FromQuery] int? voucherId = null, 
        [FromQuery] string note = "", 
        [FromQuery] int addressId = -99,
        [FromQuery] bool? usePoints = false,
        [FromQuery] bool? bopis = false)
    {
        //bopis = buy online pick in store
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

        bool isBopis = bopis == true;
        if (isBopis)
        {
            shippingFee = 0;
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

        int pointsUsed = 0;
        Customer? customerForPoints = null;
        if (customerId != -1 && usePoints == true)
        {
            customerForPoints = await _customerRepository.GetByIdAsync(customerId);
            if (customerForPoints != null && customerForPoints.TotalPoints > 0)
            {
                decimal tempFinal = totalPrice - discountAmount + shippingFee;
                pointsUsed = (int)Math.Min((decimal)customerForPoints.TotalPoints, Math.Max(0, tempFinal));
            }
        }

        if (isBopis)
        {
            var pickup = await GetOrCreatePickupAddressAsync(customerId);
            if (pickup == null)
            {
                return BadRequest("Không thể tạo địa chỉ nhận tại cửa hàng");
            }
            address = pickup;
            addressId = pickup.Id;
        }
        else if (checkoutParam.AddressDTO != null && addressId == -99)
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

        bool isZeroOrder = (totalPrice - discountAmount + shippingFee - pointsUsed) <= 0;
        int ordCode = new Random().Next(1, int.MaxValue);
        Order order = new Order();
        try
        {
            var customerUserName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerUserName))
            {
                customerUserName = User.FindFirst(ClaimTypes.Email)?.Value;
            }
            if (string.IsNullOrEmpty(customerUserName))
            {
                customerUserName = "Guest";
            }

            var statusHistory = ParseStatusHistory(order.StatusHistory);
            if (isZeroOrder)
            {
                statusHistory.Add(new StatusHistoryEntry
                {
                    Index = statusHistory.Count + 1,
                    Status = Constant.OrderStatus.Confirmed,
                    OrderStatus = Constant.OrderStatus.Confirmed,
                    PaymentStatus = Constant.PaymentStatus.Completed,
                    DateTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy"),
                    UpdatedBy = customerUserName,
                    Reasons = "Thanh toán thành công (Đơn hàng 0đ)"
                });
            }
            else if (PaymentMethodTypeId == 1)
            {
                statusHistory.Add(new StatusHistoryEntry
                {
                    Index = statusHistory.Count + 1,
                    Status = Constant.OrderStatus.Processing,
                    OrderStatus = Constant.OrderStatus.Pending,
                    PaymentStatus = Constant.PaymentStatus.Pending,
                    DateTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy"),
                    UpdatedBy = customerUserName,
                    Reasons = "Tạo đơn (COD)"
                });
            }
            else
            {
                statusHistory.Add(new StatusHistoryEntry
                {
                    Index = statusHistory.Count + 1,
                    Status = Constant.OrderStatus.WaitingForPayment,
                    OrderStatus = Constant.OrderStatus.WaitingForPayment,
                    PaymentStatus = Constant.PaymentStatus.Pending,
                    DateTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy"),
                    UpdatedBy = customerUserName,
                    Reasons = "Tạo đơn (Chờ thanh toán)"
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
                order.PaymentStatus = Constant.PaymentStatus.Completed;
                order.OrderStatus = Constant.OrderStatus.Confirmed;
            }
            else
            {
                order.PaymentStatus = Constant.PaymentStatus.Pending;
                order.OrderStatus = PaymentMethodTypeId == 2 ? Constant.OrderStatus.WaitingForPayment : Constant.OrderStatus.Pending;
            }
            order.Status = Constant.OrderStatus.GetStatusInt(order.OrderStatus);
            order.VoucherId = voucherId;
            order.InsertedAt = DateTime.Now;
            order.LastUpdate = DateTime.Now;
            order.UpdateBy = customerUserName;
            order.IsOrderPOS = false;
            order.Delete = false;
            order.BOPIS = bopis;
            order.CustomerId = customerId;
            order.CustomerType = customerId != -1 ? Constant.CustomerType.RegisteredOrder : Constant.CustomerType.GuestOrder;
            order.PaymentMethodId = PaymentMethodTypeId ?? 2;

            var result = await _orderRepository.AddAsync(order);
            if (result == null)
            {
                return BadRequest("Không thể tạo đơn hàng");
            }

            if (pointsUsed > 0 && customerForPoints != null)
            {
                customerForPoints.TotalPoints -= pointsUsed;
                await _customerRepository.UpdateAsync(customerForPoints);

                var pointHistory = new PointHistory
                {
                    CustomerId = customerForPoints.Id,
                    OrderId = result.Id,
                    Points = -pointsUsed,
                    TransactionType = "Dùng điểm",
                    Description = $"Dùng điểm cho đơn hàng {result.OrderCode}",
                    CreatedAt = DateTime.Now
                };
                await _pointHistoryRepository.AddAsync(pointHistory);
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
            int payOsAmount = (int)(totalPrice - discountAmount + shippingFee - pointsUsed);

            string cancelUrl = "http://localhost:5001/orders/payment-cancelled?orderId=" + order.Id;
            string returnUrl = "http://localhost:5001/orders/payment-success?orderId=" + order.Id + "&pos=false";

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

    [HttpPost("POS-Checkout")]
    public async Task<ActionResult<UserOrderDetailDto>> PosCheckout([FromBody] PosCheckoutDto dto)
    {
        if (dto == null || dto.ListItemCheckout == null || !dto.ListItemCheckout.Any())
        {
            return BadRequest("Dữ liệu checkout không hợp lệ");
        }

        var variantDict = new Dictionary<int, ProductVariant>();
        foreach (var item in dto.ListItemCheckout)
        {
            if (item.ProductVariantId <= 0 || item.Quantity <= 0)
            {
                return BadRequest("Sản phẩm bạn đã chọn đã hết hoặc số lượng không hợp lệ");
            }

            var variant = await _productVariantRepository.GetVariantWithDetailsByIdAsync(item.ProductVariantId);
            if (variant == null || variant.Status == -1)
            {
                return BadRequest("Sản phẩm không còn hoạt động hoặc đã bị xóa");
            }

            if (variant.StockQuantity < item.Quantity)
            {
                return BadRequest($"Số lượng kho của biến thể '{variant.VariantName}' không đủ");
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

        foreach (var product in dto.ListItemCheckout)
        {
            var variant = variantDict[product.ProductVariantId];
            
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

        int customerId = -1;
        Address address = null!;
        int addressId = 0;
        decimal shippingFee = 0;

        if (dto.CustomerId.HasValue && dto.CustomerId.Value > 0)
        {
            var customer = await _customerRepository.GetByIdAsync(dto.CustomerId.Value);
            if (customer is null || customer.Status <= 0)
            {
                return BadRequest("Khách hàng không hợp lệ");
            }

            customerId = customer.Id;
        }

        if (dto.IsShipping && dto.AddressId.HasValue && dto.AddressId.Value > 0)
        {
            address = await _addressRepository.GetByIdAsync(dto.AddressId.Value);
            if (address == null)
            {
                return BadRequest("Địa chỉ giao hàng không tồn tại");
            }
            addressId = address.Id;
            if (customerId <= 0)
            {
                customerId = address.CustomerId;
            }

            var toDistrictCode = address.Ward?.District?.Code;
            var toWardCode = address.Ward?.Code;

            if (!string.IsNullOrEmpty(toDistrictCode) && !string.IsNullOrEmpty(toWardCode))
            {
                var feeItems = orderItemDetails.Select(i => new FeeItemDTO
                {
                    Name = i.Variant.VariantName ?? i.Variant.Product?.ProductName ?? "Sản phẩm",
                    Quantity = i.Quantity,
                    Length = 30,
                    Width = 40,
                    Height = 5,
                    Weight = 400
                }).ToList();

                shippingFee = await CalculateShippingFeeAsync(toDistrictCode, toWardCode, feeItems);
            }
        }
        else if (dto.IsShipping && dto.AddressDTO != null)
        {
            var newAddress = new Address
            {
                CustomerId = customerId > 0 ? customerId : -1,
                FullName = dto.AddressDTO.FullName,
                PhoneNumber = dto.AddressDTO.PhoneNumber,
                WardId = dto.AddressDTO.WardId,
                DetailInfo = dto.AddressDTO.DetailInfo,
                IsDefault = false,
                Status = 1,
                InsertedAt = DateTime.Now,
                Delete = false
            };

            var savedAddress = await _addressRepository.AddAsync(newAddress);
            if (savedAddress == null)
            {
                return BadRequest("Không thể tạo địa chỉ giao hàng cho khách vãng lai");
            }

            address = await _addressRepository.GetByIdAsync(savedAddress.Id) ?? savedAddress;
            addressId = address.Id;

            var toDistrictCode = address.Ward?.District?.Code;
            var toWardCode = address.Ward?.Code;

            if (!string.IsNullOrEmpty(toDistrictCode) && !string.IsNullOrEmpty(toWardCode))
            {
                var feeItems = orderItemDetails.Select(i => new FeeItemDTO
                {
                    Name = i.Variant.VariantName ?? i.Variant.Product?.ProductName ?? "Sản phẩm",
                    Quantity = i.Quantity,
                    Length = 30,
                    Width = 40,
                    Height = 5,
                    Weight = 400
                }).ToList();

                shippingFee = await CalculateShippingFeeAsync(toDistrictCode, toWardCode, feeItems);
            }
        }
        else
        {
            shippingFee = 0;
            var dummyAddress = await GetOrCreatePickupAddressAsync(customerId > 0 ? customerId : -1);
            if (dummyAddress == null)
            {
                return BadRequest("Không thể tạo địa chỉ mua tại quầy");
            }
            address = dummyAddress;
            addressId = dummyAddress.Id;
        }

        decimal discountAmount = 0;
        if (dto.VoucherId.HasValue)
        {
            if (customerId == -1)
            {
                return BadRequest("Khách vãng lai không được áp dụng voucher.");
            }

            var (discount, errorMsg) = await ApplyDiscount(totalPrice, dto.VoucherId.Value, customerId, address?.PhoneNumber);
            if (errorMsg != null)
            {
                return BadRequest(errorMsg);
            }
            discountAmount = discount;
        }

        int pointsUsed = 0;
        Customer? customerForPoints = null;
        if (customerId != -1 && dto.UsePoints == true)
        {
            customerForPoints = await _customerRepository.GetByIdAsync(customerId);
            if (customerForPoints != null && customerForPoints.TotalPoints > 0)
            {
                decimal tempFinal = totalPrice - discountAmount + shippingFee;
                pointsUsed = (int)Math.Min((decimal)customerForPoints.TotalPoints, Math.Max(0, tempFinal));
            }
        }

        int ordCode = new Random().Next(1, int.MaxValue);
        Order order = new Order();
        try
        {
            bool isShipping = (dto.AddressId.HasValue && dto.AddressId.Value > 0) || dto.AddressDTO != null;
            string initialStatus = isShipping ? Constant.OrderStatus.Pending : Constant.OrderStatus.Done;

            var customerUserName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerUserName))
            {
                customerUserName = User.FindFirst(ClaimTypes.Email)?.Value;
            }
            if (string.IsNullOrEmpty(customerUserName))
            {
                customerUserName = "Guest";
            }

            var statusHistory = ParseStatusHistory(order.StatusHistory);
            statusHistory.Add(new StatusHistoryEntry
            {
                Index = statusHistory.Count + 1,
                Status = initialStatus,
                OrderStatus = initialStatus,
                PaymentStatus = Constant.PaymentStatus.Completed,
                DateTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy"),
                UpdatedBy = customerUserName,
                Reasons = initialStatus == Constant.OrderStatus.Done ? "Mua tại quầy - Hoàn thành" : "Mua tại quầy(Giao đi) - Chờ xác nhận"
            });
            order.StatusHistory = JsonSerializer.Serialize(statusHistory, _camelCaseJsonOptions);

            order.OrderCode = "DH" + ordCode.ToString();
            order.AddressId = addressId;
            order.Notes = dto.Note;
            order.ShippingFee = shippingFee;
            order.OrderDate = DateTime.Now;
            order.PaymentStatus = Constant.PaymentStatus.Completed;
            order.OrderStatus = initialStatus;
            order.Status = Constant.OrderStatus.GetStatusInt(order.OrderStatus);
            order.VoucherId = dto.VoucherId;
            order.InsertedAt = DateTime.Now;
            order.LastUpdate = DateTime.Now;
            order.UpdateBy = customerUserName;
            order.IsOrderPOS = true;
            order.Delete = false;

            order.CustomerId = customerId;
            order.CustomerType = customerId != -1 ? Constant.CustomerType.RegisteredOrder : Constant.CustomerType.GuestOrder;
            order.PaymentMethodId = dto.PaymentMethodTypeId ?? 1;

            var result = await _orderRepository.AddAsync(order);
            if (result == null)
            {
                return BadRequest("Không thể tạo đơn hàng");
            }

            if (pointsUsed > 0 && customerForPoints != null)
            {
                customerForPoints.TotalPoints -= pointsUsed;
                await _customerRepository.UpdateAsync(customerForPoints);

                var pointHistory = new PointHistory
                {
                    CustomerId = customerForPoints.Id,
                    OrderId = result.Id,
                    Points = -pointsUsed,
                    TransactionType = "Dùng điểm",
                    Description = $"Dùng điểm cho đơn hàng {result.OrderCode}",
                    CreatedAt = DateTime.Now
                };
                await _pointHistoryRepository.AddAsync(pointHistory);
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

            foreach (var product in dto.ListItemCheckout)
            {
                var decreased = await _productVariantRepository.DecreaseStockAsync(product.ProductVariantId, product.Quantity);
                if (!decreased)
                {
                    return BadRequest("Số lượng kho không đủ hoặc sản phẩm không tồn tại");
                }
            }

            if (dto.VoucherId != null && discountAmount > 0)
            {
                var voucher = await _voucherRepository.GetByIdAsync((int)dto.VoucherId);
                if (voucher != null)
                {
                    voucher.UsedCount++;
                    await _voucherRepository.UpdateAsync(voucher);
                }
            }

            var finalOrder = await _orderRepository.GetOrderDetailByIdAsync(result.Id);
            if (finalOrder == null)
            {
                return BadRequest("Không thể tải thông tin đơn hàng đã tạo");
            }

            if (customerId != -1)
            {
                await ProcessRewardPointsAsync(finalOrder);
            }

            var orderDetailDto = MapToUserOrderDetailDto(finalOrder);
            return Ok(orderDetailDto);
        }
        catch (Exception)
        {
            return StatusCode(500, Constant.ErrorCode.OtherError);
        }
    }

    [HttpGet("PaymentSuccess")]
    public async Task<ActionResult<PaymentCallbackOrderDto>> PaymentSuccess([FromQuery] int orderId, [FromQuery] bool pos, [FromQuery] string? errorMessage = null)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại");
            }

            bool isNewPaymentSuccess = order.PaymentStatus != Constant.PaymentStatus.Completed;

            var customerUserName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerUserName))
            {
                customerUserName = User.FindFirst(ClaimTypes.Email)?.Value;
            }
            if (string.IsNullOrEmpty(customerUserName))
            {
                customerUserName = "Guest";
            }

            var statusHistory = ParseStatusHistory(order.StatusHistory);
            if (pos)
            {
                statusHistory.Add(new StatusHistoryEntry
                {
                    Index = statusHistory.Count + 1,
                    Status = Constant.OrderStatus.Done,
                    OrderStatus = Constant.OrderStatus.Done,
                    PaymentStatus = Constant.PaymentStatus.Completed,
                    DateTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy"),
                    UpdatedBy = customerUserName,
                    Reasons = "Thanh toán thành công (POS)"
                });

                order.PaymentStatus = Constant.PaymentStatus.Completed;
                order.OrderStatus = Constant.OrderStatus.Done;
            }
            else
            {
                statusHistory.Add(new StatusHistoryEntry
                {
                    Index = statusHistory.Count + 1,
                    Status = Constant.OrderStatus.Pending,
                    OrderStatus = Constant.OrderStatus.Pending,
                    PaymentStatus = Constant.PaymentStatus.Completed,
                    DateTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy"),
                    UpdatedBy = customerUserName,
                    Reasons = "Thanh toán thành công"
                });

                order.PaymentStatus = Constant.PaymentStatus.Completed;
                order.OrderStatus = Constant.OrderStatus.Pending;
            }
            order.Status = Constant.OrderStatus.GetStatusInt(order.OrderStatus);

            order.StatusHistory = JsonSerializer.Serialize(statusHistory, _camelCaseJsonOptions);
            order.LastUpdate = DateTime.Now;
            order.UpdateBy = customerUserName;

            var result = await _orderRepository.UpdateAsync(order);
            if (result == null)
            {
                return BadRequest("Không thể cập nhật đơn hàng");
            }

            if (isNewPaymentSuccess)
            {
                await ProcessRewardPointsAsync(order);
            }

            return Ok(MapToPaymentCallbackDto(result));
        }
        catch (Exception)
        {
            return StatusCode(500, Constant.ErrorCode.OtherError);
        }
    }

    private async Task ProcessRewardPointsAsync(Order order)
    {
        if (order.CustomerId == -1)
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

            decimal totalSpent = await _orderRepository.GetTotalSpentInLast6MonthsAsync(customer.Id);

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

    [HttpGet("PaymentCanceled")]
    public async Task<ActionResult<PaymentCallbackOrderDto>> PaymentCanceled([FromQuery] int orderId, [FromQuery] string? errorMessage = null)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return NotFound("Đơn hàng không tồn tại");
            }

            if (order.OrderStatus == Constant.OrderStatus.Cancelled)
            {
                return Ok(MapToPaymentCallbackDto(order));
            }

            var customerUserName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerUserName))
            {
                customerUserName = User.FindFirst(ClaimTypes.Email)?.Value;
            }
            if (string.IsNullOrEmpty(customerUserName))
            {
                customerUserName = "Guest";
            }

            var statusHistory = ParseStatusHistory(order.StatusHistory);
            statusHistory.Add(new StatusHistoryEntry
            {
                Index = statusHistory.Count + 1,
                Status = Constant.OrderStatus.Cancelled,
                OrderStatus = Constant.OrderStatus.Cancelled,
                PaymentStatus = Constant.PaymentStatus.Cancelled,
                DateTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy"),
                UpdatedBy = customerUserName,
                Reasons = "Hủy thanh toán"
            });

            order.StatusHistory = JsonSerializer.Serialize(statusHistory, _camelCaseJsonOptions);
            order.PaymentStatus = Constant.PaymentStatus.Cancelled;
            order.OrderStatus = Constant.OrderStatus.Cancelled;
            order.Status = Constant.OrderStatus.GetStatusInt(order.OrderStatus);
            order.LastUpdate = DateTime.Now;
            order.UpdateBy = customerUserName;

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

            await _pointHistoryRepository.RefundPointsForOrderAsync(order.Id);

            return Ok(MapToPaymentCallbackDto(result));
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

        if (voucher.MinimumSpend != null)
        {
            decimal totalSpent = await _orderRepository.GetTotalSpentInLast6MonthsAsync(customerId);
            if (totalSpent < voucher.MinimumSpend.Value)
            {
                return (0, "Chi tiêu tích lũy trong 6 tháng gần nhất của bạn chưa đủ điều kiện để sử dụng voucher này");
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

    [HttpPost("user/cancel/{orderId}")]
    [Authorize]
    public async Task<ActionResult<UserOrderDetailDto>> CancelUserOrder(int orderId, [FromBody] CancelOrderRequestDto? request)
    {
        try
        {
            var claimVal = User.FindFirst(ClaimTypes.SerialNumber)?.Value;
            if (string.IsNullOrEmpty(claimVal) || !int.TryParse(claimVal, out var customerId))
            {
                return Unauthorized("Không thể xác định thông tin người dùng từ token.");
            }

            if (request is null || string.IsNullOrWhiteSpace(request.CancelReason))
            {
                return BadRequest("Vui lòng chọn lý do hủy đơn.");
            }

            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null || order.Delete || order.CustomerId != customerId)
            {
                return NotFound("Đơn hàng không tồn tại hoặc bạn không có quyền truy cập.");
            }

            if (order.OrderStatus == Constant.OrderStatus.Cancelled
                || order.Status == Constant.OrderStatus.GetStatusInt(Constant.OrderStatus.Cancelled))
            {
                return Ok(MapToUserOrderDetailDto(order));
            }

            if (!CanCustomerCancelOrder(order))
            {
                return BadRequest("Chỉ có thể hủy đơn khi chưa được xác nhận");
            }

            var customerUserName = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(customerUserName))
            {
                customerUserName = User.FindFirst(ClaimTypes.Email)?.Value;
            }
            if (string.IsNullOrEmpty(customerUserName))
            {
                customerUserName = "Guest";
            }

            var cancelReason = request.CancelReason.Trim();
            var cancelDetail = request.CancelDetail?.Trim();
            var reasons = string.IsNullOrWhiteSpace(cancelDetail)
                ? cancelReason
                : $"{cancelReason}: {cancelDetail}";

            var statusHistory = ParseStatusHistory(order.StatusHistory);
            statusHistory.Add(new StatusHistoryEntry
            {
                Index = statusHistory.Count + 1,
                Status = Constant.OrderStatus.Cancelled,
                OrderStatus = Constant.OrderStatus.Cancelled,
                PaymentStatus = Constant.PaymentStatus.Cancelled,
                DateTime = DateTime.Now.ToString("HH:mm dd/MM/yyyy"),
                UpdatedBy = customerUserName,
                Reasons = reasons
            });

            order.StatusHistory = JsonSerializer.Serialize(statusHistory, _camelCaseJsonOptions);
            order.PaymentStatus = Constant.PaymentStatus.Cancelled;
            order.OrderStatus = Constant.OrderStatus.Cancelled;
            order.Status = Constant.OrderStatus.GetStatusInt(order.OrderStatus);
            order.LastUpdate = DateTime.Now;
            order.UpdateBy = customerUserName;

            var result = await _orderRepository.UpdateAsync(order);
            if (result == null)
            {
                return BadRequest("Không thể hủy đơn hàng");
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

            await _pointHistoryRepository.RefundPointsForOrderAsync(order.Id);

            var refreshed = await _orderRepository.GetOrderDetailForUserAsync(orderId, customerId)
                ?? result;

            return Ok(MapToUserOrderDetailDto(refreshed));
        }
        catch (Exception)
        {
            return StatusCode(500, Constant.ErrorCode.OtherError);
        }
    }

    private static bool CanCustomerCancelOrder(Order order)
    {
        var statusCode = order.Status ?? Constant.OrderStatus.GetStatusInt(order.OrderStatus);
        if (statusCode is 1 or 2 or 3)
        {
            return true;
        }

        return order.OrderStatus is Constant.OrderStatus.Pending
            or Constant.OrderStatus.Processing
            or Constant.OrderStatus.WaitingForPayment
            or "Pending"
            or "Processing"
            or "WaitingForPayment";
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

    [HttpGet("lookup")]
    [AllowAnonymous]
    public async Task<ActionResult<List<UserOrderDetailDto>>> LookupOrders(
        [FromQuery] string? orderCode,
        [FromQuery] string? name,
        [FromQuery] string? phoneNumber)
    {
        try
        {
            int? customerId = null;
            if (User.Identity?.IsAuthenticated == true)
            {
                var claimVal = User.FindFirst(ClaimTypes.SerialNumber)?.Value;
                if (!string.IsNullOrEmpty(claimVal) && int.TryParse(claimVal, out var parsedCustomerId))
                {
                    customerId = parsedCustomerId;
                }
            }

            var hasFilter = !string.IsNullOrWhiteSpace(orderCode)
                || !string.IsNullOrWhiteSpace(name)
                || !string.IsNullOrWhiteSpace(phoneNumber);

            if (!customerId.HasValue && !hasFilter)
            {
                return BadRequest("Vui lòng cung cấp ít nhất một thông tin tra cứu (mã đơn hàng, tên hoặc số điện thoại).");
            }

            var orders = await _orderRepository.LookupOrdersAsync(
                orderCode,
                name,
                phoneNumber,
                customerId);

            var dtos = orders.Select(MapToUserOrderDetailDto).ToList();
            return Ok(dtos);
        }
        catch (Exception)
        {
            return StatusCode(500, Constant.ErrorCode.OtherError);
        }
    }

    private static PaymentCallbackOrderDto MapToPaymentCallbackDto(Order order)
    {
        return new PaymentCallbackOrderDto
        {
            Id = order.Id,
            OrderCode = order.OrderCode,
            PaymentStatus = order.PaymentStatus,
            OrderStatus = order.OrderStatus
        };
    }

    private UserOrderDetailDto MapToUserOrderDetailDto(Order order)
    {
        decimal totalPrice = order.OrderItems?.Sum(oi => oi.Subtotal) ?? 0;
        decimal discountAmount = 0;

        var pointsHistoryEntry = order.PointHistories?
            .FirstOrDefault(ph => ph.Points < 0 && ph.TransactionType == "Dùng điểm");
        int pointsUsed = pointsHistoryEntry != null ? Math.Abs(pointsHistoryEntry.Points) : 0;
        decimal pointsDiscount = pointsUsed;

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
                "Pending" or Constant.PaymentStatus.Pending => "Chờ thanh toán",
                "Completed" or Constant.PaymentStatus.Completed => "Đã thanh toán",
                "Cancelled" or Constant.PaymentStatus.Cancelled => "Đã hủy",
                _ => order.PaymentStatus
            },
            OrderStatus = order.OrderStatus switch
            {
                "Pending" or Constant.OrderStatus.Pending => "Chờ xác nhận",
                "WaitingForPayment" or Constant.OrderStatus.WaitingForPayment => "Chờ thanh toán",
                "Confirmed" or Constant.OrderStatus.Confirmed => "Đã xác nhận",
                "Done" or Constant.OrderStatus.Done => "Hoàn thành",
                "Cancelled" or Constant.OrderStatus.Cancelled => "Đã hủy",
                "Expired" or Constant.OrderStatus.Expired => "Hết hạn thanh toán",
                _ => order.OrderStatus
            },
            Status = order.Status,
            PaymentLink = order.PaymentLink,
            PaymentExpiration = order.PaymentExpiration,
            IsOrderPOS = order.IsOrderPOS,
            BOPIS = order.BOPIS,
            CustomerType = order.CustomerType,
            ReceiverName = order.Address?.FullName ?? "",
            ReceiverPhone = order.Address?.PhoneNumber ?? "",
            FullAddress = fullAddress,
            TotalPrice = totalPrice,
            DiscountAmount = discountAmount,
            PointsUsed = pointsUsed,
            PointsDiscount = pointsDiscount,
            FinalPrice = Math.Max(0, totalPrice - discountAmount - pointsDiscount + order.ShippingFee),
            StatusHistory = order.StatusHistory,
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

    private async Task<Address?> GetOrCreatePickupAddressAsync(int customerId)
    {
        return await _addressRepository.EnsureSystemPickupAsync(customerId);
    }

    private async Task<decimal> CalculateShippingFeeAsync(string toDistrictCode, string toWardCode, List<FeeItemDTO> items)
    {
        int from_district_id = 3440;
        string from_ward_code = "13007";
        var token = _configuration["GHN:Token"];
        var shopId = _configuration["GHN:ShopId"];

        if (items == null || items.Count == 0)
        {
            items = new List<FeeItemDTO>
            {
                new FeeItemDTO
                {
                    Name = "Hàng hóa",
                    Quantity = 2,
                    Length = 30,
                    Width = 40,
                    Height = 5,
                    Weight = 400
                }
            };
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
            to_district_code = toDistrictCode,
            to_ward_code = toWardCode,
            length = 30,
            width = 40,
            height = 5,
            weight = totalWeight,
            insurance_value = 0,
            coupon = (string?)null,
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
            using var doc = JsonDocument.Parse(result);
            if (doc.RootElement.TryGetProperty("data", out var dataProp) &&
                dataProp.TryGetProperty("total", out var totalProp))
            {
                return totalProp.GetDecimal();
            }
        }
        catch (Exception)
        {
        }
        return 0;
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
