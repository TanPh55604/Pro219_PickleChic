namespace PickleChic.API.DTOs;

public class RankCreateDto
{
    public string RankName { get; set; } = null!;
    public int MinPoints { get; set; }
}

public class RankUpdateDto : RankCreateDto
{
    public int Id { get; set; }
}
