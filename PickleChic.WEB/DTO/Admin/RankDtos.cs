namespace PickleChic.WEB.DTO.Admin
{
    public class RankResponse
    {
        public int Id { get; set; }

        public string RankName { get; set; } = string.Empty;

        public decimal SpendAmount { get; set; }

        public bool Delete { get; set; }
    }

    public class RankCreateRequest
    {
        public string RankName { get; set; } = string.Empty;

        public decimal SpendAmount { get; set; }
    }

    public class RankUpdateRequest : RankCreateRequest
    {
        public int Id { get; set; }
    }

    public class PercentRewardResponse
    {
        public double PercentReward { get; set; }
    }

    public class PercentRewardUpdateRequest
    {
        public double PercentReward { get; set; }
    }
}
