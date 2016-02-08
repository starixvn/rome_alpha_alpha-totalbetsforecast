namespace Alpha.TotalBetsForecast.Site.Services.Impl
{
    using System.Linq;
    using Criteria;
    using DataAccesses;
    using Fanex.Common.Helper;
    using Models;
    using Models.Requests;

    public class ForecastService : IForecastService
    {
        private IForecastDataAccess _forecastDataAccess;

        public ForecastService(IForecastDataAccess forecastDataAccess)
        {
            _forecastDataAccess = forecastDataAccess;
        }

        public MatchForecast GetMatchForecast(MatchForecastQuery query)
        {
            Check.Argument.IsNotNull(query, nameof(query));

            var matchForecastRaw = _forecastDataAccess.GetMatchForecast(new GetMatchForecastCriteria
            {
                CustId = query.CustId,
                IsLive = query.IsLive,
                LiveAwayScore = query.LiveAwayScore,
                LiveHomeScore = query.LiveHomeScore,
                MatchId = query.MatchId
            });

            var betTypeForecasts = matchForecastRaw.Item1.Select(
                betTypeForecast =>
                {
                    var winlossColumnName = matchForecastRaw.Item2.First(winLossColumnName => winLossColumnName.BetTypeId == betTypeForecast.BetTypeId);
                    betTypeForecast.FirstColumnName = winlossColumnName.FirstColumnName;
                    betTypeForecast.SecondColumnName = winlossColumnName.SecondColumnName;
                    betTypeForecast.ThirdColumnName = winlossColumnName.ThirdColumnName;

                    return betTypeForecast;
                });

            return new MatchForecast
            {
                BetTypeForecasts = betTypeForecasts,
                ForecastTotal = matchForecastRaw.Item3
            };
        }
    }
}