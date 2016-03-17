namespace Alpha.TotalBetsForecast.Site.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Criteria;
    using DataAccesses;
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
        public IEnumerable<MatchForecast> GetMatchForecasts(MatchForecastQuery query, DateTime kickOffTime)
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

            if (!matchForecastRaw.Item1.Any())
            {
                return Enumerable.Empty<MatchForecast>();
            }

            var matches = matchForecastRaw.Item1;
            var betTypeForecastDtos = matchForecastRaw.Item2;
            var winLossColumnNameDtos = matchForecastRaw.Item3;
            var matchForecasts = new List<MatchForecast>();

            //// Remove haft-time forecast when now is second half
            if (kickOffTime.AddMinutes(HalftimeMinutes) < DateTime.Now)
            {
                betTypeForecastDtos = betTypeForecastDtos.Where(betTypeForcastDto => IsFullTimeBetType(betTypeForcastDto.BetTypeId));
            }

            foreach (var match in matches)
            {
                var betTypes = betTypeForecastDtos.Where(betTypeDto => betTypeDto.MatchId == match.Id);
                var winLossNames = winLossColumnNameDtos.Where(winLossName => winLossName.MatchId == match.Id);
                var betTypeForecasts = new List<BetTypeForecast>();

                foreach (var betType in betTypes)
                {
                    betTypeForecasts.Add(BuildBetTypeForecast(betType, winLossColumnNameDtos.First(winLossName => winLossName.BetTypeId == betType.BetTypeId));
                }

                matchForecasts.Add(new MatchForecast
                {
                    MatchInfo = match,
                    BetTypeForecasts = betTypeForecasts
                });
            }

            return matchForecasts;
        }

        private BetTypeForecast BuildBetTypeForecast(
            BetTypeForecastDto betTypeForecastDto,
            WinLossColumnNameDto winLossColumnNameDto)
        {
            var betTypeForecast = new BetTypeForecast
            {
                BetTypeId = betTypeForecastDto.BetTypeId,
                BetCount = winLossColumnNameDto.BetCount,
                Turnover = winLossColumnNameDto.Turnover
            };

            BuildWinLossNameAndValue(betTypeForecast, betTypeForecastDto, winLossColumnNameDto);

            return betTypeForecast;
        }

        private void BuildWinLossNameAndValue(
            BetTypeForecast betTypeForecast,
            BetTypeForecastDto betTypeForecastDto,
            WinLossColumnNameDto winLossColumnNameDto)
        {
            var winLossColumnNames = new List<string>();
            var winLossValues = new List<decimal>();

            if (betTypeForecast.IsOneXTwoBetType)
            {
                winLossValues.Add(betTypeForecastDto.WinLossFirstColumn);
                winLossValues.Add(betTypeForecastDto.WinLossSecondColumn);
                winLossValues.Add(betTypeForecastDto.WinLossThirdColumn);

                winLossColumnNames.Add(winLossColumnNameDto.FirstColumnName);
                winLossColumnNames.Add(winLossColumnNameDto.SecondColumnName);
                winLossColumnNames.Add(winLossColumnNameDto.ThirdColumnName);
            }
            else
            {
                winLossValues.Add(betTypeForecastDto.WinLossFirstColumn);
                winLossValues.Add(betTypeForecastDto.WinLossSecondColumn);
                winLossValues.Add(betTypeForecastDto.WinLossThirdColumn);
                winLossValues.Add(betTypeForecastDto.WinLossFourthColumn);
                winLossValues.Add(betTypeForecastDto.WinLossFifthColumn);

                winLossColumnNames.Add(winLossColumnNameDto.FirstColumnName);
                winLossColumnNames.Add(winLossColumnNameDto.SecondColumnName);
                winLossColumnNames.Add(winLossColumnNameDto.ThirdColumnName);
                winLossColumnNames.Add(winLossColumnNameDto.FourthColumnName);
                winLossColumnNames.Add(winLossColumnNameDto.FifthColumnName);
            }

            betTypeForecast.WinLossValues = winLossValues;
            betTypeForecast.WinLossColumnNames = winLossColumnNames;
        }

        private bool IsFullTimeBetType(int betTypeId)
        {
            return betTypeId == (int)BetType.Handicap
                || betTypeId == (int)BetType.Over_Under
                || betTypeId == (int)BetType.FT_1X2;
        }
    }
}