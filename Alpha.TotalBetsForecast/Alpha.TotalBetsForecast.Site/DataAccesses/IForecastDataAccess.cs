namespace Alpha.TotalBetsForecast.Site.DataAccesses
{
    using System;
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Models;
    using Models.Dtos;

    public interface IForecastDataAccess
    {
        Tuple<IEnumerable<Match>, IEnumerable<BetTypeForecastDto>, IEnumerable<WinLossColumnNameDto>> GetMatchForecast(ICriteria criteria);
    }
}