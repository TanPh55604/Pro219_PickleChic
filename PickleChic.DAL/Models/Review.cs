using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PickleChic.DAL.Models;

public class Review
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(OrderItem))]
    public int OrderItemId { get; set; }

    [ForeignKey(nameof(ProductVariant))]
    public int ProductVariantId { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }

    [Required]
    public string Content { get; set; } = null!;

    public int Overall { get; set; }

    public int Status { get; set; } = 1;

    public DateTime CreateAt { get; set; } = DateTime.Now;

    public bool Delete { get; set; } = false;

    public OrderItem? OrderItem { get; set; }

    public ProductVariant? ProductVariant { get; set; }
}
