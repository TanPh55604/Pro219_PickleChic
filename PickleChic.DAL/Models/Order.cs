using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PickleChic.DAL.Models;

public class Order
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Customer))]
    public int CustomerId { get; set; }

    public string OrderCode { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    [ForeignKey(nameof(ShippingAddress))]
    public int ShippingAddressId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal DiscountAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal FinalAmount { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ShippingFee { get; set; }

    [ForeignKey(nameof(PaymentMethod))]
    public int PaymentMethodId { get; set; }

    [ForeignKey(nameof(Voucher))]
    public int? VoucherId { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public string OrderStatus { get; set; } = null!;

    public string? Notes { get; set; }

    public string? StatusHistory { get; set; }

    public string? CustomerType { get; set; }

    public bool InOrderPOS { get; set; }

    public string? PaymentLink { get; set; }

    public DateTime? PaymentExpiration { get; set; }

    public string CustomerName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Ward { get; set; } = null!;

    public string District { get; set; } = null!;

    public string Province { get; set; } = null!;

    public DateTime? LastUpdate { get; set; }

    public string? UpdateBy { get; set; }

    public DateTime InsertedAt { get; set; }

    public bool Delete { get; set; }

    public DateTime? DeleteAt { get; set; }

    public Customer? Customer { get; set; }

    public Address? ShippingAddress { get; set; }

    public PaymentMethod? PaymentMethod { get; set; }

    public Voucher? Voucher { get; set; }

    public ICollection<OrderItem>? OrderItems { get; set; }

    public ICollection<PointHistory>? PointHistories { get; set; }
}
