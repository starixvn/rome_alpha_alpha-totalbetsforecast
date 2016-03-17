namespace Alpha.TotalBetsForecast.Site.Models.Views
{
    using System;
    using System.Collections.Generic;

    public class LeagueForecastViewModel
    {
        public string LeagueName { get; set; }

        public IEnumerable<MatchForecast> MatchForecasts { get; set; }
    }
}