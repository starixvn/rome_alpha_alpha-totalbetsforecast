namespace Alpha.TotalBetsForecast.Site.Services.Impl
{
    using System.Collections.Generic;
    using System.Linq;
    using Criteria;
    using DataAccesses;
    using Fanex.Common.Helper;
    using Helpers;
    using Models;
    using Models.Requests;
    using Sunplus.Agent.Model.BL.Definitions;

    public class ForecastService : IForecastService
    {
        private const string HomeChoice = "h";
        private const string AwayChoice = "a";
        private const string OverChoice = "o";
        private const string UnderChoice = "u"; 

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

                var betTypeForecast = new BetTypeForecast
                {
                    BetTypeId = betTypeDto.BetTypeId,
                    BetType = betTypeDto.BetType
                };

                var groupedBetTypeForecastDto = matchForecastRaw.Item1.Where(betTypeForecastDto => betTypeForecastDto.BetTypeId == betTypeDto.BetTypeId);
                var betChoices = new List<BetChoiceForecast>();

                if (groupedBetTypeForecastDto.Any())
                {
                    groupedBetTypeForecastDto = groupedBetTypeForecastDto.OrderBy(betTypeGrouped => betTypeGrouped.BetChoice, new BetChoiceComparer());
                    var isFirstBetChoice = true;
                    foreach (var betTypeItem in groupedBetTypeForecastDto)
                    {
                        var winLossColumnNames = new List<string>();
                        var winLossValues = new List<decimal>();
                        var betChoice = new BetChoiceForecast
                        {
                            BetChoice = betTypeItem.BetChoice,
                            BetCount = betTypeItem.BetCount,
                            Turnover = betTypeItem.Turnover
                        };

                        if (betTypeForecast.IsOneXTwoBetType)
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

                        betChoice.WinLossValues = winLossValues;
                        betChoice.WinLossColumnNames = winLossColumnNames;

                        if (betTypeForecast.IsOneXTwoBetType)
                        {
                            betChoice.HandicapValue = betTypeItem.BetChoice;
                        }
                        else if (betTypeForecast.IsOverUnder 
                            && betTypeItem.BetChoice.ToLower() == AwayChoice)
                        {
                            betChoice.HandicapValue = UnderChoice;
                        }
                        else
                        {
                            betChoice.HandicapValue = betTypeItem.BetType;
                        }

                        betChoices.Add(betChoice);
                    }
                }

                processedBetTypeIds.Add(betTypeDto.BetTypeId);

                betTypeForecast.BetChoices = betChoices;
                betTypeForecasts.Add(betTypeForecast);
            }

            return new MatchForecast
            {
                BetTypeForecasts = betTypeForecasts,
                ForecastTotal = matchForecastRaw.Item3
            };
        }
    }
}