namespace Alpha.TotalBetsForecast.Site.Services.Impl
{
    using System.Collections.Generic;
    using System.Linq;
    using Criteria;
    using DataAccesses;
    using Fanex.Common.Helper;
    using Models;
    using Models.Requests;
    using Sunplus.Agent.Model.BL.Definitions;

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

            var processedBetTypeIds = new List<int>();
            var betTypeForecasts = new List<BetTypeForecast>();

            foreach (var betTypeDto in matchForecastRaw.Item1)
            {
                if (processedBetTypeIds.Contains(betTypeDto.BetTypeId))
                {
                    continue;
                }

                var groupedBetTypeForecastDto = matchForecastRaw.Item1.Where(betTypeForecastDto => betTypeForecastDto.BetTypeId == betTypeDto.BetTypeId);
                var betChoices = new List<BetChoiceForecast>();

                if (groupedBetTypeForecastDto.Any())
                {
                    var isFirstBetChoice = true;
                    foreach (var betTypeItem in groupedBetTypeForecastDto)
                    {
                        var winLossColumnNames = new List<string>();
                        var winLossValues = new List<decimal>();

                        if (betTypeItem.BetTypeId == (int)BetType.FT_1X2
                            || betTypeItem.BetTypeId == (int)BetType.FirstHalf_1X2)
                        {
                            winLossValues.Add(betTypeItem.WinLossFirstColumn);
                        }
                        else
                        {
                            if (isFirstBetChoice)
                            {
                                var winlossColumnNameDto = matchForecastRaw.Item2.First(winLossColumnName => winLossColumnName.BetTypeId == betTypeItem.BetTypeId);
                                winLossColumnNames.Add(winlossColumnNameDto.FirstColumnName);
                                winLossColumnNames.Add(winlossColumnNameDto.SecondColumnName);
                                winLossColumnNames.Add(winlossColumnNameDto.ThirdColumnName);
                                isFirstBetChoice = false;
                            }

                            winLossValues.Add(betTypeItem.WinLossFirstColumn);
                            winLossValues.Add(betTypeItem.WinLossSecondColumn);
                            winLossValues.Add(betTypeItem.WinLossThirdColumn);
                        }

                        betChoices.Add(new BetChoiceForecast
                        {
                            BetChoice = betTypeItem.BetChoice,
                            BetCount = betTypeItem.BetCount,
                            Turnover = betTypeItem.Turnover,
                            WinLossColumnNames = winLossColumnNames,
                            WinLossValues = winLossValues
                        });
                    }
                }

                processedBetTypeIds.Add(betTypeDto.BetTypeId);

                betTypeForecasts.Add(new BetTypeForecast
                {
                    BetTypeId = betTypeDto.BetTypeId,
                    BetType = betTypeDto.BetType,
                    BetChoices = betChoices
                });
            }

            return new MatchForecast
            {
                BetTypeForecasts = betTypeForecasts,
                ForecastTotal = matchForecastRaw.Item3
            };
        }
    }
}