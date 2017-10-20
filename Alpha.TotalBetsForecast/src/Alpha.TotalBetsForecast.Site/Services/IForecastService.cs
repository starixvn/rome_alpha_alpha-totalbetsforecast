namespace Alpha.TotalBetsForecast.Site.Services
{
    using System.Collections.Generic;
    using Models;
    using Models.Requests;

    public interface IForecastService
    {
        IEnumerable<MatchForecast> GetMatchForecasts(MatchForecastsQuery query);
    }
}