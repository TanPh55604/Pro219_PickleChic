using System;
using System.Collections.Generic;

namespace PickleChic.API.DTOs;

public class UserOrderDetailDto
{
    public int Id { get; set; }
    public string OrderCode { get; set; } = null!;
    public DateTime OrderDate { get; set; }
    public decimal ShippingFee { get; set; }
    public string Notes { get; set; } = "";
    public string PurchaseChannel { get; set; } = null!;
    public string PaymentStatus { get; set; } = null!;
    public string OrderStatus { get; set; } = null!;
    public string? PaymentLink { get; set; }
    public int? Status { get; set; } = 0!;
    public DateTime? PaymentExpiration { get; set; }
    public bool IsOrderPOS { get; set; }
    public bool? BOPIS { get; set; }
    public string? CustomerType { get; set; }
    
    public string ReceiverName { get; set; } = null!;
    public string ReceiverPhone { get; set; } = null!;
    public string FullAddress { get; set; } = null!;
    
    public decimal TotalPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalPrice { get; set; }

    public List<UserOrderItemDetailDto> OrderItems { get; set; } = new();

    public string? StatusHistory { get; set; }
}

public class UserOrderItemDetailDto
{
    public int ProductVariantId { get; set; }
    public string ProductName { get; set; } = null!;
    public string VariantName { get; set; } = null!;
    public List<string> Attributes { get; set; } = new();
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Subtotal { get; set; }
}

public class CancelOrderRequestDto
{
    public string CancelReason { get; set; } = null!;
    public string? CancelDetail { get; set; }
}

public class GuestCancelOrderRequestDto
{
    public string OrderCode { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string CancelReason { get; set; } = null!;
    public string? CancelDetail { get; set; }
}

public class PaymentCallbackOrderDto
{
    public int Id { get; set; }
    public string OrderCode { get; set; } = null!;
    public string PaymentStatus { get; set; } = null!;
    public string OrderStatus { get; set; } = null!;
}
