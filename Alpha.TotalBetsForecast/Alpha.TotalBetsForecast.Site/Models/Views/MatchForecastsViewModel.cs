namespace Alpha.TotalBetsForecast.Site.Models.Views
{
    using System.Collections.Generic;

    public class MatchForecastsViewModel
    {
        public string LeagueName { get; set; }

        public IEnumerable<MatchForecastViewModel> MatchForecasts { get; set; }
    }
}