namespace Alpha.TotalBetsForecast.Site.Models.Dtos
{
    public class WinLossColumnNameDto
    {
        public int MatchId { get; set; }

        public int BetTypeId { get; set; }

        public int BetCount { get; set; }

        public decimal Turnover { get; set; }

        public string FirstColumnName { get; set; }

        public string SecondColumnName { get; set; }

        public string ThirdColumnName { get; set; }

        public string FourthColumnName { get; set; }

        public string FifthColumnName { get; set; }
    }
}