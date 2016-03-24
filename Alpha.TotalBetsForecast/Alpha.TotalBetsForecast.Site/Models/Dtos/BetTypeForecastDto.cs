namespace Alpha.TotalBetsForecast.Site.Models.Dtos
{
    public class BetTypeForecastDto
    {
        public int MatchId { get; set; }

        public int BetTypeId { get; set; }

        public decimal WinLossFirstColumn { get; set; }

        public decimal WinLossSecondColumn { get; set; }

        public decimal WinLossThirdColumn { get; set; }

        public decimal WinLossFourthColumn { get; set; }

        public decimal WinLossFifthColumn { get; set; }
    }
}