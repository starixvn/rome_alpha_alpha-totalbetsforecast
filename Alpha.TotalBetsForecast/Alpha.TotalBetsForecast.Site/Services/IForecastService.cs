namespace Alpha.TotalBetsForecast.Site.Services
{
    using System;
    using Models;
    using Models.Requests;

    public interface IForecastService
    {
        MatchForecast GetMatchForecast(MatchForecastQuery query, DateTime kickOffTime);
    }
}