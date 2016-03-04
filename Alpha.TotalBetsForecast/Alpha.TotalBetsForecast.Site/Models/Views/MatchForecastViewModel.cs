namespace Alpha.TotalBetsForecast.Site.Models.Views
{
    using System;
    using System.Collections.Generic;

    public class MatchForecastViewModel
    {
        public int MatchId { get; set; }

        public string LeagueName { get; set; }

        public bool HasLive { get; set; }

        public bool IsLive { get; set; }

        public string KickOffTimeString { get; set; }

        public string HomeName { get; set; }

        public string AwayName { get; set; }

        public byte LiveHomeScore { get; set; }

        public byte LiveAwayScore { get; set; }

        public decimal ForecastTotal { get; set; }

        public IEnumerable<BetTypeForecast> BetTypeForecasts { get; set; }

        public int MatchRowSpan { get; set; }
    }
}