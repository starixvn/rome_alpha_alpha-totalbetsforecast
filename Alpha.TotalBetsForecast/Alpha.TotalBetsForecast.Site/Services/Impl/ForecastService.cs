namespace Alpha.TotalBetsForecast.Site.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Criteria;
    using DataAccesses;
    using Fanex.Common.Helper;
    using Helpers;
    using Models;
    using Models.Dtos;
    using Models.Requests;
    using Sunplus.Agent.Model.BL.Definitions;

    public class ForecastService : IForecastService
    {
        private const string HomeChoice = "h";
        private const string AwayChoice = "a";
        private const string OverChoice = "o";
        private const string UnderChoice = "u";
        private const int HalftimeMinutes = 45;

        private readonly IForecastDataAccess _forecastDataAccess;

        public ForecastService(IForecastDataAccess forecastDataAccess)
        {
            _forecastDataAccess = forecastDataAccess;
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Reviewed!")]
        public MatchForecast GetMatchForecast(MatchForecastQuery query, DateTime kickOffTime)
        {
            Check.Argument.IsNotNull(query, nameof(query));

            var matchForecastRaw = _forecastDataAccess.GetMatchForecast(new GetMatchForecastCriteria
            {
                CustId = query.CustId,
                IsLive = query.IsLive,
                LiveAwayScore = query.LiveAwayScore,
                LiveHomeScore = query.LiveHomeScore,
                MatchId = query.MatchId,
                KickOffTime = kickOffTime
            });

            var betTypeForecastDtos = matchForecastRaw.Item1;
            var winLossColumnNameDtos = matchForecastRaw.Item2;

            //// Remove haft-time forecast when now is second half
            if (kickOffTime.AddMinutes(HalftimeMinutes) < DateTime.Now)
            {
                betTypeForecastDtos = betTypeForecastDtos.Where(betTypeForcastDto => IsFullTimeBetType(betTypeForcastDto.BetTypeId));
            }

            var processedBetTypeIds = new List<int>();
            var betTypeForecasts = new List<BetTypeForecast>();

            foreach (var betTypeDto in betTypeForecastDtos)
            {
                if (processedBetTypeIds.Contains(betTypeDto.BetTypeId))
                {
                    //// Processed, then continue bet type dto
                    continue;
                }

                var betTypeForecast = BuildBetTypeForecast(betTypeForecastDtos, winLossColumnNameDtos, betTypeDto);

                betTypeForecasts.Add(betTypeForecast);

                processedBetTypeIds.Add(betTypeDto.BetTypeId);
            }

            return new MatchForecast
            {
                BetTypeForecasts = betTypeForecasts,
                ForecastTotal = matchForecastRaw.Item3
            };
        }

        private BetTypeForecast BuildBetTypeForecast(
            IEnumerable<BetTypeForecastDto> betTypeForecastDtos,
            IEnumerable<WinLossColumnNameDto> winLossColumnNameDtos,
            BetTypeForecastDto betTypeDto)
        {
            var betTypeForecast = new BetTypeForecast
            {
                BetTypeId = betTypeDto.BetTypeId,
                BetType = betTypeDto.BetType
            };

            var groupedBetTypeForecastDto = betTypeForecastDtos.Where(
                betTypeForecastDto => betTypeForecastDto.BetTypeId == betTypeDto.BetTypeId);

            var betChoices = new List<BetChoiceForecast>();

            if (groupedBetTypeForecastDto.Any())
            {
                groupedBetTypeForecastDto = groupedBetTypeForecastDto.OrderBy(
                    betTypeGrouped => betTypeGrouped.BetChoice,
                    new BetChoiceComparer());

                var isFirstBetChoice = true;

                foreach (var betTypeItem in groupedBetTypeForecastDto)
                {
                    var betChoice = new BetChoiceForecast
                    {
                        BetChoice = betTypeItem.BetChoice,
                        BetCount = betTypeItem.BetCount,
                        Turnover = betTypeItem.Turnover
                    };

                    BuildWinLossNameAndValue(
                        winLossColumnNameDtos,
                        betChoice,
                        betTypeItem,
                        betTypeForecast.IsOneXTwoBetType,
                        ref isFirstBetChoice);

                    betChoice.HandicapValue = CalculateHandicapValue(betTypeForecast, betTypeItem);

                    betChoices.Add(betChoice);
                }
            }

            betTypeForecast.BetChoices = betChoices;

            return betTypeForecast;
        }

        private void BuildWinLossNameAndValue(
            IEnumerable<WinLossColumnNameDto> winLossColumnNameDtos,
            BetChoiceForecast betChoice,
            BetTypeForecastDto betTypeItem,
            bool isOneXTwo,
            ref bool isFirstBetChoice)
        {
            var winLossColumnNames = new List<string>();
            var winLossValues = new List<decimal>();

            if (isOneXTwo)
            {
                winLossValues.Add(betTypeItem.WinLossFirstColumn);
            }
            else
            {
                if (isFirstBetChoice)
                {
                    var winlossColumnNameDto = winLossColumnNameDtos.First(
                        winLossColumnName => winLossColumnName.BetTypeId == betTypeItem.BetTypeId);

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
        }

        private string CalculateHandicapValue(BetTypeForecast betTypeForecast, BetTypeForecastDto betTypeItem)
        {
            if (betTypeForecast.IsOneXTwoBetType)
            {
                return betTypeItem.BetChoice;
            }

            if (betTypeForecast.IsOverUnder
                && betTypeItem.BetChoice.ToLower() == AwayChoice)
            {
                return UnderChoice;
            }

            return betTypeItem.BetType;
        }

        private bool IsFullTimeBetType(int betTypeId)
        {
            return betTypeId == (int)BetType.Handicap
                || betTypeId == (int)BetType.Over_Under
                || betTypeId == (int)BetType.FT_1X2;
        }
    }
}