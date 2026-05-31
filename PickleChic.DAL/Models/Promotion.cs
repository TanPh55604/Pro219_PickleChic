using System.ComponentModel.DataAnnotations;

namespace PickleChic.DAL.Models;

public class Promotion
{
    [Key]
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    public ICollection<PromotionDetail>? PromotionDetails { get; set; }

    public ICollection<OrderItem>? OrderItems { get; set; }
}
