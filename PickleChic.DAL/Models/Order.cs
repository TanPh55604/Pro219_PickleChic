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

    [ForeignKey(nameof(Address))]
    public int AddressId { get; set; }

    public DateTime OrderDate { get; set; }

    [ForeignKey(nameof(PaymentMethod))]
    public int PaymentMethodId { get; set; }

    [ForeignKey(nameof(Voucher))]
    public int? VoucherId { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public string OrderStatus { get; set; } = null!;

    public string? Notes { get; set; }

    public DateTime? LastUpdate { get; set; }

    public bool Delete { get; set; }

    public string? CustomerType { get; set; }

    public bool IsOrderPOS { get; set; }

    public bool? BOPIS { get; set; } = false;

    public string? PaymentLink { get; set; }

    public DateTime? PaymentExpiration { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal ShippingFee { get; set; }

    public string? StatusHistory { get; set; }

    public string? UpdateBy { get; set; }

    public int? Status { get; set; }

    public bool StockDeducted { get; set; } = false;

    [Column(TypeName = "decimal(18,2)")]
    public decimal? PaidAmount { get; set; }

    public DateTime InsertedAt { get; set; }

    public DateTime? DeleteAt { get; set; }

    public Customer? Customer { get; set; }

    public Address? Address { get; set; }

    public PaymentMethod? PaymentMethod { get; set; }

    public Voucher? Voucher { get; set; }

    public ICollection<OrderItem>? OrderItems { get; set; }

    public ICollection<PointHistory>? PointHistories { get; set; }
}
