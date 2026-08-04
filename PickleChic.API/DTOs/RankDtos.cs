namespace PickleChic.API.DTOs;

public class RankCreateDto
{
    public string RankName { get; set; } = null!;
    public decimal SpendAmount { get; set; }
}

public class RankUpdateDto : RankCreateDto
{
    public int Id { get; set; }
}
