namespace Alpha.TotalBetsForecast.Site.DataAccesses.Impl
{
    using System;
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Models;
    using Models.Dtos;

    public class ForecastDataAccess : IForecastDataAccess
    {
        public Tuple<IEnumerable<Match>, IEnumerable<BetTypeForecastDto>, IEnumerable<WinLossColumnNameDto>> GetMatchForecast(ICriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");

            using (IObjectDb objectDb = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var queryMultiple = objectDb.QueryMultiple(criteria.GetSpParams()))
                {
                    var matches = queryMultiple.Read<Match>();

                    var betTypeForecasts = queryMultiple.Read<BetTypeForecastDto>();

                    var winlossColumnNames = queryMultiple.Read<WinLossColumnNameDto>();

                    return Tuple.Create(matches, betTypeForecasts, winlossColumnNames);
                }
            }
        }
    }
}