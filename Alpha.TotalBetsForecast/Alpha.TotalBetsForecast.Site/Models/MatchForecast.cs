namespace Alpha.TotalBetsForecast.Site.Models
{
    using System.Collections.Generic;

    public class MatchForecast
    {
        public Match MatchInfo { get; set; }

        public IEnumerable<BetTypeForecast> BetTypeForecasts { get; set; }
    }
}