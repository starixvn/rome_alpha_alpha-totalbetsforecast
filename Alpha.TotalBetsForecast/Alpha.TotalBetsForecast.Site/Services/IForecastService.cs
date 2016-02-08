namespace Alpha.TotalBetsForecast.Site.Services
{
    using Models;
    using Models.Requests;

    public interface IForecastService
    {
        MatchForecast GetMatchForecast(MatchForecastQuery query);
    }
}