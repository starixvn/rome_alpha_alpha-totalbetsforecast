namespace Alpha.TotalBetsForecast.Site.Models.Dtos
{
    public class WinLossColumnNameDto
    {
        public int MatchId { get; set; }

        public int BetTypeId { get; set; }

        public int BetCount { get; set; }

        public decimal Turnover { get; set; }

        public decimal FirstColumnName { get; set; }

        public decimal SecondColumnName { get; set; }

        public decimal ThirdColumnName { get; set; }

        public decimal FourthColumnName { get; set; }

        public decimal FifthColumnName { get; set; }
    }
}