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
                MatchIds = query.MatchIds
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

                if (query.IsLive &&
                    match.KickOffTime.AddMinutes(HalftimeMinutes) < DateTime.Now)
                {
                    betTypes = betTypes.Where(betType => IsFullTimeBetType(betType.BetTypeId));
                }

                var betTypeForecasts = new List<BetTypeForecast>();
                BetTypeForecast handicapBetType = null;

                foreach (var betType in betTypes)
                {
                    var winlossName = winLossNames.First(winLossName => winLossName.BetTypeId == betType.BetTypeId);
                    var betTypeForecast = BuildBetTypeForecast(betType, winlossName);

                    if (betTypeForecast.IsHandicap)
                    {
                        handicapBetType = betTypeForecast;
                    }

                    match.TotalBetCount += betTypeForecast.BetCount;
                    match.TotalTurnover += betTypeForecast.Turnover;
                    match.TotalForecastWinLoss += CalculateTotalForecastWinLoss(query.IsLive, match, betTypeForecast, handicapBetType);

                    betTypeForecasts.Add(betTypeForecast);
                }

                match.BetTypeForecasts = betTypeForecasts.OrderBy(betType => betType.BetTypeId);

                matchForecasts.Add(match);
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

            var winLossColumnNames = new List<string>();
            var winLossValues = new List<decimal>();

            if (betTypeForecast.IsOneXTwo)
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

            betTypeForecast.WinLossValues = winLossValues;
            betTypeForecast.WinLossColumnNames = winLossColumnNames;

            return betTypeForecast;
        }

        private decimal CalculateTotalForecastWinLoss(bool isLive, Match match, BetTypeForecast betType, BetTypeForecast handicapBetType)
        {
            switch ((BetType)betType.BetTypeId)
            {
                case BetType.Handicap:
                case BetType._1st_Handicap:
                    return betType.WinLossValues.ElementAt(2);

                case BetType.Over_Under:
                case BetType._1st_Over_Under:
                    return betType.WinLossValues.ElementAt(4);
                case BetType.FT_1X2:
                case BetType.FirstHalf_1X2:
                    if (isLive)
                    {
                        if (match.LiveHomeScore == match.LiveAwayScore)
                        {
                            return betType.WinLossValues.ElementAt(1);
                        }

                        if (match.LiveHomeScore > match.LiveAwayScore)
                        {
                            return betType.WinLossValues.ElementAt(0);
                        }

                        return betType.WinLossValues.ElementAt(2);
                    }

                    if (handicapBetType != null)
                    {
                        var currentHdp = Converter.ToInt(handicapBetType.WinLossColumnNames.ElementAt(2));

                        if (currentHdp == 0)
                        {
                            return betType.WinLossValues.ElementAt(1);
                        }

                        if (currentHdp > 0)
                        {
                            return betType.WinLossValues.ElementAt(0);
                        }

                        return betType.WinLossValues.ElementAt(1);
                    }

                    return 0;

                default:
                    return 0;
            }
        }

        private bool IsFullTimeBetType(int betTypeId)
        {
            return betTypeId == (int)BetType.Handicap
                || betTypeId == (int)BetType.Over_Under
                || betTypeId == (int)BetType.FT_1X2;
        }
    }
}