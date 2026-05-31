using System.ComponentModel.DataAnnotations;

namespace PickleChic.DAL.Models;

public class Rank
{
    [Key]
    public int Id { get; set; }

    public string RankName { get; set; } = null!;

    public int MinPoints { get; set; }

    public ICollection<Customer>? Customers { get; set; }
}
