namespace Alpha.TotalBetsForecast.Site.Models.Dtos
{
    public class WinLossColumnNameDto
    {
        public int MatchId { get; set; }

        public int BetTypeId { get; set; }

        public int BetCount { get; set; }

        public decimal Turnover { get; set; }

        public int FirstColumnName { get; set; }

        public int SecondColumnName { get; set; }

        public int ThirdColumnName { get; set; }

        public int FourthColumnName { get; set; }

        public int FifthColumnName { get; set; }

        public int HighlightValueOverUnder { get; set; }
    }
}