namespace Alpha.TotalBetsForecast.Site.Models
{
    using System.Collections.Generic;

    public class MatchForecast
    {
        public Match Match { get; set; }

        public League League { get; set; }

        public IEnumerable<BetTypeForecast> BetTypeForecasts { get; set; }

        public decimal ForecastTotal { get; set; }
    }
}