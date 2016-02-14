namespace Alpha.TotalBetsForecast.Site.Models
{
    using System.Collections.Generic;

    public class MatchForecast
    {
        public IEnumerable<BetTypeForecast> BetTypeForecasts { get; set; }

        public decimal ForecastTotal { get; set; }
    }
}