namespace Alpha.TotalBetsForecast.Site.DataAccesses
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Models.Dtos;

    public interface IForecastDataAccess
    {
        Tuple<IEnumerable<BetTypeForecastDto>, IEnumerable<WinLossColumnNameDto>, decimal> GetMatchForecast(ICriteria criteria);
    }
}