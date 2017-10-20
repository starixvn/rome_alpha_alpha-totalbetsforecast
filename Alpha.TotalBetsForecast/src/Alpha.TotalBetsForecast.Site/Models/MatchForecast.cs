namespace Alpha.TotalBetsForecast.Site.Models
{
    using System.Collections.Generic;

    public class MatchForecast : Match
    {
        public IEnumerable<BetTypeForecast> BetTypeForecasts { get; set; }

        public int TotalBetCount { get; set; }

        public decimal TotalTurnover { get; set; }

        public decimal TotalForecastWinLoss { get; set; }
    }
}