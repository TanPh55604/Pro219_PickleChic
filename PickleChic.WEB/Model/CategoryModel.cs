namespace PickleChic.WEB.Model;

public class CategoryModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? LinkImage { get; set; }

    public string? Description { get; set; }

    public string? UpdateBy { get; set; }

    public bool Status { get; set; } = true;

    public DateTime CreateAt { get; set; } = DateTime.Now;

    public DateTime? UpdateAt { get; set; }

    public DateTime? DeleteAt { get; set; }
}