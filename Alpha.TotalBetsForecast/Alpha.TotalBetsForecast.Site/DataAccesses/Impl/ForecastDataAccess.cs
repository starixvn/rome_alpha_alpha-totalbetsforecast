namespace Alpha.TotalBetsForecast.Site.DataAccesses.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Models;
    using Models.Dtos;

    public class ForecastDataAccess : IForecastDataAccess
    {
        public Tuple<IEnumerable<BetTypeForecast>, IEnumerable<WinLossColumnNameDto>, decimal> GetMatchForecast(ICriteria criteria)
        {
            using (IObjectDb objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var queryMultiple = objectDb.QueryMultiple(criteria.GetSpParams()))
                {
                    var betTypeForecasts = queryMultiple.Read<BetTypeForecast>();

                    var winlossColumnNames = queryMultiple.Read<WinLossColumnNameDto>();

                    var forecastTotal = queryMultiple.Read<decimal>().FirstOrDefault();

                    return Tuple.Create(betTypeForecasts, winlossColumnNames, forecastTotal);
                }
            }
        }
    }
}