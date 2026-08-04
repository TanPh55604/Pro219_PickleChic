using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PickleChic.DAL.Models;

public class Rank
{
    [Key]
    public int Id { get; set; }

    public string RankName { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal SpendAmount { get; set; }

    public ICollection<Customer>? Customers { get; set; }
}
