namespace Alpha.TotalBetsForecast.Site.Services
{
    using System;
    using System.Collections.Generic;
    using Models;
    using Models.Requests;

    public interface IForecastService
    {
        IEnumerable<MatchForecast> GetMatchForecasts(MatchForecastQuery query);
    }
}