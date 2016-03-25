namespace Alpha.TotalBetsForecast.Site.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Criteria;
    using DataAccesses;
    using Fanex.Common;
    using Fanex.Common.Helper;
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
        public IEnumerable<MatchForecast> GetMatchForecasts(MatchForecastsQuery query)
        {
            Check.Argument.IsNotNull(query, nameof(query));

            var matchForecastRaw = _forecastDataAccess.GetMatchForecast(new GetMatchForecastsCriteria
            {
                CustId = query.CustId,
                MatchIds = query.MatchIds,
                IsLive = query.IsLive
            });

            if (!matchForecastRaw.Item1.Any())
            {
                return Enumerable.Empty<MatchForecast>();
            }

            var matches = matchForecastRaw.Item1;
            var betTypeForecastDtos = matchForecastRaw.Item2;
            var winLossColumnNameDtos = matchForecastRaw.Item3;
            var matchForecasts = new List<MatchForecast>();

            foreach (var match in matches)
            {
                var betTypes = betTypeForecastDtos.Where(betTypeDto => betTypeDto.MatchId == match.Id);
                var winLossNames = winLossColumnNameDtos.Where(winLossName => winLossName.MatchId == match.Id);

                matchForecasts.Add(BuildMatchForecast(match, betTypes, winLossNames));
            }

            return matchForecasts;
        }

        private MatchForecast BuildMatchForecast(
            MatchForecast match,
            IEnumerable<BetTypeForecastDto> betTypes,
            IEnumerable<WinLossColumnNameDto> winLossNames)
        {
            if (match.IsLive
                && match.KickOffTime.AddMinutes(HalftimeMinutes) < DateTime.Now)
            {
                betTypes = betTypes.Where(betType => IsFullTimeBetType(betType.BetTypeId));
            }

            var betTypeForecasts = new List<BetTypeForecast>();
            BetTypeForecast handicapBetType = null;
            var totalBetType = betTypes.Count();

            for (int i = 0; i < totalBetType; i++)
            {
                var betType = betTypes.ElementAt(i);
                var winlossName = winLossNames.First(winLossName => winLossName.BetTypeId == betType.BetTypeId);
                var betTypeForecast = BuildBetTypeForecast(betType, winlossName, match, handicapBetType);

                if (betTypeForecast.IsHandicap)
                {
                    handicapBetType = betTypeForecast;
                }

                match.TotalBetCount += betTypeForecast.BetCount;
                match.TotalTurnover += betTypeForecast.Turnover;

                if (match.IsLive)
                {
                    match.TotalForecastWinLoss += betTypeForecast.WinLossValues.ElementAt(betTypeForecast.MainWinLossIndex);
                }

                betTypeForecasts.Add(betTypeForecast);
            }

            match.BetTypeForecasts = betTypeForecasts.OrderBy(betType => betType.BetTypeId);

            return match;
        }

        private BetTypeForecast BuildBetTypeForecast(
            BetTypeForecastDto betTypeForecastDto,
            WinLossColumnNameDto winLossColumnNameDto,
            MatchForecast matchForecast,
            BetTypeForecast handicapBetType)
        {
            var betTypeForecast = new BetTypeForecast
            {
                BetTypeId = betTypeForecastDto.BetTypeId,
                BetCount = winLossColumnNameDto.BetCount,
                Turnover = winLossColumnNameDto.Turnover
            };

            var winLossColumnNames = new List<string>();
            var winLossValues = new List<decimal>();

            AddWinLossNameAndValue(
                betTypeForecast.IsOneXTwo,
                winLossColumnNames,
                winLossValues,
                betTypeForecastDto,
                winLossColumnNameDto);

            var totalWinLossName = winLossColumnNames.Count;

            for (int i = 0; i < totalWinLossName; i++)
            {
                if (IsMainScore(
                    betTypeForecast,
                    matchForecast,
                    i,
                    handicapBetType,
                    winLossColumnNameDto.HighlightValueOverUnder,
                    winLossColumnNames.ElementAt(i)))
                {
                    betTypeForecast.MainWinLossIndex = i;
                    break;
                }
            }

            betTypeForecast.WinLossValues = winLossValues;
            betTypeForecast.WinLossColumnNames = winLossColumnNames;

            return betTypeForecast;
        }

        private void AddWinLossNameAndValue(
            bool isOneXTwo,
            List<string> winLossColumnNames,
            List<decimal> winLossValues,
            BetTypeForecastDto betTypeForecastDto,
            WinLossColumnNameDto winLossColumnNameDto)
        {
            if (isOneXTwo)
            {
                winLossValues.Add(betTypeForecastDto.WinLossFirstColumn);
                winLossValues.Add(betTypeForecastDto.WinLossSecondColumn);
                winLossValues.Add(betTypeForecastDto.WinLossThirdColumn);

                winLossColumnNames.Add("1");
                winLossColumnNames.Add("X");
                winLossColumnNames.Add("2");
            }
            else
            {
                winLossValues.Add(betTypeForecastDto.WinLossFirstColumn);
                winLossValues.Add(betTypeForecastDto.WinLossSecondColumn);
                winLossValues.Add(betTypeForecastDto.WinLossThirdColumn);
                winLossValues.Add(betTypeForecastDto.WinLossFourthColumn);
                winLossValues.Add(betTypeForecastDto.WinLossFifthColumn);

                winLossColumnNames.Add(winLossColumnNameDto.FirstColumnName.ToString());
                winLossColumnNames.Add(winLossColumnNameDto.SecondColumnName.ToString());
                winLossColumnNames.Add(winLossColumnNameDto.ThirdColumnName.ToString());
                winLossColumnNames.Add(winLossColumnNameDto.FourthColumnName.ToString());
                winLossColumnNames.Add(winLossColumnNameDto.FifthColumnName.ToString());
            }
        }

        private bool IsMainScore(
            BetTypeForecast betTypeForecast,
            MatchForecast matchForecast,
            int winlossIndex,
            BetTypeForecast handicapBetType,
            int highlightValueOverUnder,
            string currentWinLossName)
        {
            if (betTypeForecast.IsHandicap && winlossIndex == 2)
            {
                return true;
            }

            if (betTypeForecast.IsOverUnder)
            {
                if (matchForecast.IsLive && winlossIndex == 4)
                {
                    return true;
                }

                if (!matchForecast.IsLive
                    && string.Compare(currentWinLossName, highlightValueOverUnder.ToString(), true) == 0)
                {
                    return true;
                }
            }

            if (betTypeForecast.IsOneXTwo)
            {
                if (matchForecast.IsLive)
                {
                    if ((matchForecast.LiveHomeScore == matchForecast.LiveAwayScore && winlossIndex == 1)
                        || (matchForecast.LiveHomeScore > matchForecast.LiveAwayScore && winlossIndex == 0)
                        || (matchForecast.LiveHomeScore < matchForecast.LiveAwayScore && winlossIndex == 2))
                    {
                        return true;
                    }
                }
                else
                {
                    if (handicapBetType != null)
                    {
                        var handicapValue = Converter.ToInt(handicapBetType.WinLossColumnNames.ElementAt(2));
                        if ((handicapValue == 0 && winlossIndex == 1)
                            || (handicapValue > 0 && winlossIndex == 0)
                            || (handicapValue < 0 && winlossIndex == 2))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool IsFullTimeBetType(int betTypeId)
        {
            return betTypeId == (int)BetType.Handicap
                || betTypeId == (int)BetType.Over_Under
                || betTypeId == (int)BetType.FT_1X2;
        }
    }
}