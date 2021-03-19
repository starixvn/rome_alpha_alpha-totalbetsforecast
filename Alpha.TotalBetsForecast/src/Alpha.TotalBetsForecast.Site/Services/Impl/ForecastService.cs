namespace Alpha.TotalBetsForecast.Site.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Alpha.Web.Helpers;
    using Criteria;
    using Fanex.Caching;
    using Fanex.Common;
    using Fanex.Common.Helper;
    using Fanex.Data.Repository;
    using Models;
    using Models.Dtos;
    using Models.Requests;
    using Sunplus.Agent.Model.BL.Definitions;

    public class ForecastService : IForecastService
    {
        private const string MatchForecastsCacheKey = "Alpha.TotalBetsForecast.Site.MatchForecastsCached_{0}_{1}_{2}";
        private const int MatchForecastsCacheTimeOut = 5; // 5 Second

        private const int HalftimeMinutes = 45;

        private readonly ICacheService cacheService;
        private readonly IDynamicRepository dynamicRepository;
        private readonly PartialUMService partialUMService;

        public ForecastService(
            ICacheService cacheService,
            IDynamicRepository dynamicRepository,
            PartialUMService partialUMService)
        {
            this.cacheService = cacheService;
            this.dynamicRepository = dynamicRepository;
            this.partialUMService = partialUMService;
        }

        public IEnumerable<MatchForecast> GetMatchForecasts(MatchForecastsQuery query)
        {
            Check.Argument.IsNotNull(query, nameof(query));
            Check.Argument.IsNotEmpty(query.MatchIds, nameof(query.MatchIds));

            var matchForecasts = cacheService.Get<IEnumerable<MatchForecast>>(GetMatchForecastsCacheKey(query));

            if (matchForecasts == null
                || !matchForecasts.Any())
            {
                matchForecasts = BuildMatchForecasts(query);

                cacheService.Set(
                    GetMatchForecastsCacheKey(query),
                    matchForecasts,
                    new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(MatchForecastsCacheTimeOut)));
            }

            return matchForecasts;
        }

        private static string GetMatchForecastsCacheKey(MatchForecastsQuery query)
        {
            return string.Format(
                MatchForecastsCacheKey,
                query.CustId,
                query.MatchIds.Replace(",", "_"),
                query.IsLive);
        }

        private IEnumerable<MatchForecast> BuildMatchForecasts(MatchForecastsQuery query)
        {
            var criteria = new GetMatchForecastsCriteria
            {
                CustId = query.CustId,
                MatchIds = query.MatchIds,
                IsLive = query.IsLive
            };

            partialUMService.CheckPartialUM(criteria.GetSettingKey());

            var matchForecastRaw = dynamicRepository.FetchMultiple<MatchForecast, BetTypeForecastDto, WinLossColumnNameDto>(criteria);

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
                var betTypes = betTypeForecastDtos.Where(betTypeDto => betTypeDto.MatchId == match.Id).OrderBy(betTypeDto => betTypeDto.BetTypeId);
                var winLossNames = winLossColumnNameDtos.Where(winLossName => winLossName.MatchId == match.Id).OrderBy(winLossName => winLossName.BetTypeId);

                matchForecasts.Add(BuildMatchForecast(match, betTypes, winLossNames));
            }

            return matchForecasts;
        }

        private MatchForecast BuildMatchForecast(
            MatchForecast match,
            IEnumerable<BetTypeForecastDto> betTypes,
            IEnumerable<WinLossColumnNameDto> winLossNames)
        {
            var fullTimeBetType = betTypes;
            if (match.IsLive
                && match.KickOffTime.AddMinutes(HalftimeMinutes) < DateTime.Now)
            {
                fullTimeBetType = betTypes.Where(betType => IsFullTimeBetType(betType.BetTypeId));
            }

            var betTypeForecasts = new List<BetTypeForecast>();
            BetTypeForecast handicapBetType = null;
            var totalBetType = fullTimeBetType.Count();

            for (int i = 0; i < totalBetType; i++)
            {
                var betType = fullTimeBetType.ElementAt(i);
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

            match.ShowTime = BuildShowTime(match.ShowTime);

            return match;
        }

        private static string BuildShowTime(string showTime)
        {
            var showTimeString = string.Empty;
            if (!string.IsNullOrWhiteSpace(showTime))
            {
                var showTimeStrings = showTime.Trim().Split(' ');
                var totalShowTimeStrings = showTimeStrings.Count();

                showTimeString = totalShowTimeStrings > 2 ? $"{string.Join(" ", showTimeStrings.Take(totalShowTimeStrings - 1))}<br />{showTimeStrings.Last()}" :
                    string.Join("<br />", showTimeStrings);
            }

            return showTimeString;
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

            var i = 0;
            var isMainScore = false;
            while (!isMainScore && i < winLossColumnNames.Count)
            {
                isMainScore = IsMainScore(
                    betTypeForecast,
                    matchForecast,
                    i,
                    handicapBetType,
                    winLossColumnNameDto.HighlightValueOverUnder,
                    winLossColumnNames.ElementAt(i));
                if (isMainScore)
                {
                    betTypeForecast.MainWinLossIndex = i;
                }
                i++;
            }

            betTypeForecast.WinLossValues = winLossValues;
            betTypeForecast.WinLossColumnNames = winLossColumnNames;

            return betTypeForecast;
        }

        private static void AddWinLossNameAndValue(
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

        private static bool IsMainScoreOfOneXTwoAndIsLive(MatchForecast matchForecast, int winlossIndex)
        {
            var indexIs0 = matchForecast.LiveHomeScore > matchForecast.LiveAwayScore && winlossIndex == 0;
            var indexIs1 = matchForecast.LiveHomeScore == matchForecast.LiveAwayScore && winlossIndex == 1;
            var indexIs2 = matchForecast.LiveHomeScore < matchForecast.LiveAwayScore && winlossIndex == 2;

            return indexIs1 || indexIs0 || indexIs2;
        }

        private static bool IsMainScoreOfOneXTwo(
           BetTypeForecast betTypeForecast,
           MatchForecast matchForecast,
           int winlossIndex,
           BetTypeForecast handicapBetType)
        {
            if (betTypeForecast.IsOneXTwo)
            {
                if (matchForecast.IsLive)
                {
                    return IsMainScoreOfOneXTwoAndIsLive(matchForecast, winlossIndex);
                }

                if (!matchForecast.IsLive && handicapBetType != null)
                {
                    var handicapValue = Converter.ToInt(handicapBetType.WinLossColumnNames.ElementAt(2));
                    var indexIs0 = handicapValue > 0 && winlossIndex == 0;
                    var indexIs1 = handicapValue == 0 && winlossIndex == 1;
                    var indexIs2 = handicapValue < 0 && winlossIndex == 2;

                    return indexIs0 || indexIs1 || indexIs2;
                }
            }

            return false;
        }

        private static bool IsMainScoreOfOverUnder(
           BetTypeForecast betTypeForecast,
           MatchForecast matchForecast,
           int winlossIndex,
           int highlightValueOverUnder,
           string currentWinLossName)
        {
            if (betTypeForecast.IsOverUnder && matchForecast.IsLive && winlossIndex == 4)
            {
                return true;
            }

            return betTypeForecast.IsOverUnder && !matchForecast.IsLive
                && currentWinLossName.Equals(highlightValueOverUnder.ToString());
        }

        private static bool IsMainScore(
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

            var isMainScoreOfOverUnder = IsMainScoreOfOverUnder(betTypeForecast, matchForecast, winlossIndex, highlightValueOverUnder, currentWinLossName);
            if (isMainScoreOfOverUnder)
            {
                return true;
            }

            return IsMainScoreOfOneXTwo(betTypeForecast, matchForecast, winlossIndex, handicapBetType);
        }

        private static bool IsFullTimeBetType(int betTypeId)
            => betTypeId == (int)BetType.Handicap ||
            betTypeId == (int)BetType.Over_Under ||
            betTypeId == (int)BetType.FT_1X2;
    }
}