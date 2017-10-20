namespace Alpha.TotalBetsForecast.Site.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Criteria;
    using DataAccesses;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Models;
    using Models.Dtos;
    using Models.Requests;
    using NSubstitute;
    using NUnit.Framework;
    using Site.Services.Impl;
    using Sunplus.Agent.Model.BL.Definitions;

    [TestFixture]
    public class ForecastServiceTests
    {
        private IForecastDataAccess _forecastDataAccess;
        private ICache _cacheManager;
        private ForecastService _forecastService;

        [SetUp]
        public void SetUp()
        {
            _forecastDataAccess = Substitute.For<IForecastDataAccess>();
            _cacheManager = Substitute.For<ICache>();

            _forecastService = new ForecastService(_forecastDataAccess, _cacheManager);
        }

        [Test]
        public void GetMatchForecast_QueryIsNull_ThrowArgumentNullException()
        {
            MatchForecastsQuery query = null;

            Assert.Throws<ArgumentNullException>(() => _forecastService.GetMatchForecasts(query));
        }

        [Test]
        public void GetMatchForecast_MatchIdsIsEmpty_ThrowArgumentException()
        {
            MatchForecastsQuery query = new MatchForecastsQuery(1);

            Assert.Throws<ArgumentException>(() => _forecastService.GetMatchForecasts(query));
        }

        [Test]
        public void GetMatchForecast_DataWereCached_ReturnCachedData()
        {
            //// Arrange
            var query = new MatchForecastsQuery(1)
            {
                IsLive = true,
                MatchIds = "1,2"
            };
            var expectedMatchForecasts = new List<MatchForecast>
                {
                    CreateMatchForecast(1, true)
                };
            _cacheManager.Get<IEnumerable<MatchForecast>>("Alpha.TotalBetsForecast.Site.MatchForecastsCached_1_1_2_True")
                .Returns(expectedMatchForecasts);

            //// Act
            var actualMatchForecast = _forecastService.GetMatchForecasts(query);

            //// Assert
            Assert.AreEqual(expectedMatchForecasts, actualMatchForecast);
        }

        [Test]
        public void GetMatchForecast_ExecuteGetMatchForecast()
        {
            //// Arrange
            var query = new MatchForecastsQuery(1)
            {
                IsLive = true,
                MatchIds = "1,2"
            };

            var forecastData = Tuple.Create(Enumerable.Empty<MatchForecast>(), Enumerable.Empty<BetTypeForecastDto>(), Enumerable.Empty<WinLossColumnNameDto>());

            _forecastDataAccess.GetMatchForecast(Arg.Any<ICriteria>()).Returns(forecastData);

            //// Act
            _forecastService.GetMatchForecasts(query);

            //// Assert
            _forecastDataAccess.Received().GetMatchForecast(
                Arg.Is<GetMatchForecastsCriteria>(
                    criteria => criteria.MatchIds == query.MatchIds
                            && criteria.IsLive
                            && criteria.CustId == 1));
        }

        [Test]
        public void GetMatchForecast_ReceivesListOfBetTypeDto_GroupDataByMatch()
        {
            //// Arrange
            var query = new MatchForecastsQuery(1)
            {
                MatchIds = "1"
            };
            IEnumerable<BetTypeForecastDto> rawData = new List<BetTypeForecastDto>
            {
                new BetTypeForecastDto { MatchId = 1, BetTypeId = (int)BetType.Handicap },
                new BetTypeForecastDto { MatchId = 1, BetTypeId = (int)BetType.Over_Under },
                new BetTypeForecastDto { MatchId = 1, BetTypeId = (int)BetType.FT_1X2 },
                new BetTypeForecastDto { MatchId = 2, BetTypeId = (int)BetType._1st_Handicap },
                new BetTypeForecastDto { MatchId = 2, BetTypeId = (int)BetType._1st_Over_Under },
                new BetTypeForecastDto { MatchId = 2, BetTypeId = (int)BetType.FirstHalf_1X2 }
            };

            IEnumerable<WinLossColumnNameDto> columnNameData = new List<WinLossColumnNameDto>
            {
                new WinLossColumnNameDto { MatchId = 1, BetTypeId = (int)BetType.Handicap },
                new WinLossColumnNameDto { MatchId = 1, BetTypeId = (int)BetType.Over_Under },
                new WinLossColumnNameDto { MatchId = 1, BetTypeId = (int)BetType.FT_1X2 },
                new WinLossColumnNameDto { MatchId = 2, BetTypeId = (int)BetType._1st_Handicap },
                new WinLossColumnNameDto { MatchId = 2, BetTypeId = (int)BetType._1st_Over_Under },
                new WinLossColumnNameDto { MatchId = 2, BetTypeId = (int)BetType.FirstHalf_1X2 }
            };

            IEnumerable<MatchForecast> matchForecasts = new List<MatchForecast>
            {
                new MatchForecast { Id = 1 },
                new MatchForecast { Id = 2 }
            };

            _forecastDataAccess.GetMatchForecast(Arg.Any<ICriteria>()).Returns(Tuple.Create(matchForecasts, rawData, columnNameData));

            //// Act
            var matchForcasts = _forecastService.GetMatchForecasts(query);

            //// Assert
            Assert.True(matchForcasts.Count() == 2
                && matchForcasts.ElementAt(0).Id == 1
                && matchForcasts.ElementAt(0).BetTypeForecasts.Count() == 3
                && matchForcasts.ElementAt(1).Id == 2
                && matchForcasts.ElementAt(1).BetTypeForecasts.Count() == 3);
        }

        [TestCase(BetType.Handicap)]
        [TestCase(BetType._1st_Handicap)]
        public void GetMatchForecast_ReceivesListOfWinLossName_BuildWinLossNameListForHandicap(BetType betType)
        {
            //// Arrange
            var query = new MatchForecastsQuery(1)
            {
                MatchIds = "1"
            };
            IEnumerable<BetTypeForecastDto> rawData = new List<BetTypeForecastDto>
            {
                CreateBetTypeForecastDto(1, (int)betType)
            };

            IEnumerable<WinLossColumnNameDto> columnNameData = new List<WinLossColumnNameDto>
            {
                CreateWinLossColumnNameDto(1, (int)betType)
            };

            IEnumerable<MatchForecast> matchForecasts = new List<MatchForecast>
            {
                CreateMatchForecast(1, false)
            };

            _forecastDataAccess.GetMatchForecast(Arg.Any<ICriteria>()).Returns(Tuple.Create(matchForecasts, rawData, columnNameData));

            //// Act
            var matchForcasts = _forecastService.GetMatchForecasts(query);

            //// Assert
            var firstMatchHandicapWinLossNames = matchForcasts.ElementAt(0).BetTypeForecasts.ElementAt(0).WinLossColumnNames;
            Assert.True(firstMatchHandicapWinLossNames.Count() == 5
                && firstMatchHandicapWinLossNames.ElementAt(0) == "1"
                && firstMatchHandicapWinLossNames.ElementAt(1) == "2"
                && firstMatchHandicapWinLossNames.ElementAt(2) == "3"
                && firstMatchHandicapWinLossNames.ElementAt(3) == "4"
                && firstMatchHandicapWinLossNames.ElementAt(4) == "5"
                && matchForcasts.ElementAt(0).BetTypeForecasts.ElementAt(0).MainWinLossIndex == 2);
        }

        [TestCase(BetType.Over_Under, 1, 0)]
        [TestCase(BetType.Over_Under, 2, 1)]
        [TestCase(BetType._1st_Over_Under, 2, 1)]
        [TestCase(BetType._1st_Over_Under, 2, 1)]
        public void GetMatchForecast_MatchIsNoneLive_BuildWinLossNameListForOverUnder(BetType betType, int highLightValue, int mainWinLossIndex)
        {
            //// Arrange
            var query = new MatchForecastsQuery(1)
            {
                MatchIds = "1"
            };
            var isLive = false;
            IEnumerable<BetTypeForecastDto> rawData = new List<BetTypeForecastDto>
            {
                CreateBetTypeForecastDto(1, (int)betType)
            };

            IEnumerable<WinLossColumnNameDto> columnNameData = new List<WinLossColumnNameDto>
            {
                CreateWinLossColumnNameDto(1, (int)betType, highLightValue)
            };

            IEnumerable<MatchForecast> matchForecasts = new List<MatchForecast>
            {
                CreateMatchForecast(1, isLive)
            };

            _forecastDataAccess.GetMatchForecast(Arg.Any<ICriteria>()).Returns(Tuple.Create(matchForecasts, rawData, columnNameData));

            //// Act
            var matchForcasts = _forecastService.GetMatchForecasts(query);

            //// Assert
            var firstMatchHandicapWinLossNames = matchForcasts.ElementAt(0).BetTypeForecasts.ElementAt(0).WinLossColumnNames;
            Assert.True(firstMatchHandicapWinLossNames.Count() == 5
                && firstMatchHandicapWinLossNames.ElementAt(0) == "1"
                && firstMatchHandicapWinLossNames.ElementAt(1) == "2"
                && firstMatchHandicapWinLossNames.ElementAt(2) == "3"
                && firstMatchHandicapWinLossNames.ElementAt(3) == "4"
                && firstMatchHandicapWinLossNames.ElementAt(4) == "5"
                && matchForcasts.ElementAt(0).BetTypeForecasts.ElementAt(0).MainWinLossIndex == mainWinLossIndex);
        }

        [TestCase(BetType.Over_Under)]
        [TestCase(BetType._1st_Over_Under)]
        public void GetMatchForecast_MatchIsLive_BuildWinLossNameListForOverUnder(BetType betType)
        {
            //// Arrange
            var query = new MatchForecastsQuery(1)
            {
                MatchIds = "1"
            };
            var isLive = true;
            IEnumerable<BetTypeForecastDto> rawData = new List<BetTypeForecastDto>
            {
                CreateBetTypeForecastDto(1, (int)betType)
            };

            IEnumerable<WinLossColumnNameDto> columnNameData = new List<WinLossColumnNameDto>
            {
                CreateWinLossColumnNameDto(1, (int)betType)
            };

            IEnumerable<MatchForecast> matchForecasts = new List<MatchForecast>
            {
                CreateMatchForecast(1, isLive)
            };

            _forecastDataAccess.GetMatchForecast(Arg.Any<ICriteria>()).Returns(Tuple.Create(matchForecasts, rawData, columnNameData));

            //// Act
            var matchForcasts = _forecastService.GetMatchForecasts(query);

            //// Assert
            var firstMatchHandicapWinLossNames = matchForcasts.ElementAt(0).BetTypeForecasts.ElementAt(0).WinLossColumnNames;
            Assert.True(firstMatchHandicapWinLossNames.Count() == 5
                && firstMatchHandicapWinLossNames.ElementAt(0) == "1"
                && firstMatchHandicapWinLossNames.ElementAt(1) == "2"
                && firstMatchHandicapWinLossNames.ElementAt(2) == "3"
                && firstMatchHandicapWinLossNames.ElementAt(3) == "4"
                && firstMatchHandicapWinLossNames.ElementAt(4) == "5"
                && matchForcasts.ElementAt(0).BetTypeForecasts.ElementAt(0).MainWinLossIndex == 4);
        }

        [TestCase(BetType.FT_1X2, 0, 0, 1)]
        [TestCase(BetType.FT_1X2, 1, 0, 0)]
        [TestCase(BetType.FT_1X2, 0, 2, 2)]
        [TestCase(BetType.FirstHalf_1X2, 0, 0, 1)]
        [TestCase(BetType.FirstHalf_1X2, 1, 0, 0)]
        [TestCase(BetType.FirstHalf_1X2, 0, 2, 2)]
        public void GetMatchForecast_MatchIsLive_BuildWinLossNameListForOneXTwo(BetType betType, byte liveHomeScore, byte liveAwayScore, int mainWinLossIndex)
        {
            //// Arrange
            var query = new MatchForecastsQuery(1)
            {
                MatchIds = "1"
            };
            var isLive = true;
            IEnumerable<BetTypeForecastDto> rawData = new List<BetTypeForecastDto>
            {
                CreateBetTypeForecastDto(1, (int)betType)
            };

            IEnumerable<WinLossColumnNameDto> columnNameData = new List<WinLossColumnNameDto>
            {
                CreateWinLossColumnNameDto(1, (int)betType)
            };

            IEnumerable<MatchForecast> matchForecasts = new List<MatchForecast>
            {
                CreateMatchForecast(1, isLive, null, liveHomeScore, liveAwayScore)
            };

            _forecastDataAccess.GetMatchForecast(Arg.Any<ICriteria>()).Returns(Tuple.Create(matchForecasts, rawData, columnNameData));

            //// Act
            var matchForcasts = _forecastService.GetMatchForecasts(query);

            //// Assert
            var firstMatchHandicapWinLossNames = matchForcasts.ElementAt(0).BetTypeForecasts.ElementAt(0).WinLossColumnNames;
            Assert.True(firstMatchHandicapWinLossNames.Count() == 3
                && firstMatchHandicapWinLossNames.ElementAt(0) == "1"
                && firstMatchHandicapWinLossNames.ElementAt(1) == "X"
                && firstMatchHandicapWinLossNames.ElementAt(2) == "2"
                && matchForcasts.ElementAt(0).BetTypeForecasts.ElementAt(0).MainWinLossIndex == mainWinLossIndex);
        }

        [TestCase(false, 0, 0, 0, 0)]
        [TestCase(true, 0, 0, 20, 10)]
        [TestCase(true, 1, 0, 18, 9)]
        [TestCase(true, 0, 1, 22, 11)]
        public void GetMatchForecast_SumWinLossOfBetTypes(bool isLive, byte liveHomeScore, byte liveAwayScore, decimal totalWinLossMatch1, decimal totalWinLossMatch2)
        {
            //// Arrange
            var query = new MatchForecastsQuery(1)
            {
                MatchIds = "1"
            };
            IEnumerable<BetTypeForecastDto> rawData = new List<BetTypeForecastDto>
            {
                CreateBetTypeForecastDto(1, (int)BetType.Handicap),
                CreateBetTypeForecastDto(1, (int)BetType.Over_Under),
                CreateBetTypeForecastDto(1, (int)BetType.FT_1X2),
                CreateBetTypeForecastDto(1, (int)BetType._1st_Handicap),
                CreateBetTypeForecastDto(1, (int)BetType._1st_Over_Under),
                CreateBetTypeForecastDto(1, (int)BetType.FirstHalf_1X2),
                CreateBetTypeForecastDto(2, (int)BetType.Handicap),
                CreateBetTypeForecastDto(2, (int)BetType.Over_Under),
                CreateBetTypeForecastDto(2, (int)BetType.FT_1X2)
            };

            IEnumerable<WinLossColumnNameDto> columnNameData = new List<WinLossColumnNameDto>
            {
                CreateWinLossColumnNameDto(1, (int)BetType.Handicap),
                CreateWinLossColumnNameDto(1, (int)BetType.Over_Under),
                CreateWinLossColumnNameDto(1, (int)BetType.FT_1X2),
                CreateWinLossColumnNameDto(1, (int)BetType._1st_Handicap),
                CreateWinLossColumnNameDto(1, (int)BetType._1st_Over_Under),
                CreateWinLossColumnNameDto(1, (int)BetType.FirstHalf_1X2),
                CreateWinLossColumnNameDto(2, (int)BetType.Handicap),
                CreateWinLossColumnNameDto(2, (int)BetType.Over_Under),
                CreateWinLossColumnNameDto(2, (int)BetType.FT_1X2)
            };

            IEnumerable<MatchForecast> matchForecasts = new List<MatchForecast>
            {
                CreateMatchForecast(1, isLive, null, liveHomeScore, liveAwayScore),
                CreateMatchForecast(2, isLive, null, liveHomeScore, liveAwayScore)
            };

            _forecastDataAccess.GetMatchForecast(Arg.Any<ICriteria>()).Returns(Tuple.Create(matchForecasts, rawData, columnNameData));

            //// Act
            var matchForcasts = _forecastService.GetMatchForecasts(query);

            //// Assert
            var match1 = matchForcasts.ElementAt(0);
            var match2 = matchForcasts.ElementAt(1);
            Assert.True(
                match1.TotalForecastWinLoss == totalWinLossMatch1
                && match2.TotalForecastWinLoss == totalWinLossMatch2);
        }

        [Test]
        public void GetMatchForecast_SumTotalBetCount()
        {
            //// Arrange
            var query = new MatchForecastsQuery(1)
            {
                MatchIds = "1"
            };
            IEnumerable<BetTypeForecastDto> rawData = new List<BetTypeForecastDto>
            {
                CreateBetTypeForecastDto(1, (int)BetType.Handicap),
                CreateBetTypeForecastDto(1, (int)BetType.Over_Under),
                CreateBetTypeForecastDto(1, (int)BetType.FT_1X2),
                CreateBetTypeForecastDto(1, (int)BetType._1st_Handicap),
                CreateBetTypeForecastDto(1, (int)BetType._1st_Over_Under),
                CreateBetTypeForecastDto(1, (int)BetType.FirstHalf_1X2),
                CreateBetTypeForecastDto(2, (int)BetType.Handicap),
                CreateBetTypeForecastDto(2, (int)BetType.Over_Under),
                CreateBetTypeForecastDto(2, (int)BetType.FT_1X2)
            };

            IEnumerable<WinLossColumnNameDto> columnNameData = new List<WinLossColumnNameDto>
            {
                CreateWinLossColumnNameDto(1, (int)BetType.Handicap),
                CreateWinLossColumnNameDto(1, (int)BetType.Over_Under),
                CreateWinLossColumnNameDto(1, (int)BetType.FT_1X2),
                CreateWinLossColumnNameDto(1, (int)BetType._1st_Handicap),
                CreateWinLossColumnNameDto(1, (int)BetType._1st_Over_Under),
                CreateWinLossColumnNameDto(1, (int)BetType.FirstHalf_1X2),
                CreateWinLossColumnNameDto(2, (int)BetType.Handicap),
                CreateWinLossColumnNameDto(2, (int)BetType.Over_Under),
                CreateWinLossColumnNameDto(2, (int)BetType.FT_1X2)
            };

            IEnumerable<MatchForecast> matchForecasts = new List<MatchForecast>
            {
                CreateMatchForecast(1),
                CreateMatchForecast(2)
            };

            _forecastDataAccess.GetMatchForecast(Arg.Any<ICriteria>()).Returns(Tuple.Create(matchForecasts, rawData, columnNameData));

            //// Act
            var matchForcasts = _forecastService.GetMatchForecasts(query);

            //// Assert
            var match1 = matchForcasts.ElementAt(0);
            var match2 = matchForcasts.ElementAt(1);
            Assert.True(
                match1.TotalBetCount == 12
                && match2.TotalBetCount == 6);
        }

        [Test]
        public void GetMatchForecast_SumTotalTurnover()
        {
            //// Arrange
            var query = new MatchForecastsQuery(1)
            {
                MatchIds = "1"
            };
            IEnumerable<BetTypeForecastDto> rawData = new List<BetTypeForecastDto>
            {
                CreateBetTypeForecastDto(1, (int)BetType.Handicap),
                CreateBetTypeForecastDto(1, (int)BetType.Over_Under),
                CreateBetTypeForecastDto(1, (int)BetType.FT_1X2),
                CreateBetTypeForecastDto(1, (int)BetType._1st_Handicap),
                CreateBetTypeForecastDto(1, (int)BetType._1st_Over_Under),
                CreateBetTypeForecastDto(1, (int)BetType.FirstHalf_1X2),
                CreateBetTypeForecastDto(2, (int)BetType.Handicap),
                CreateBetTypeForecastDto(2, (int)BetType.Over_Under),
                CreateBetTypeForecastDto(2, (int)BetType.FT_1X2)
            };

            IEnumerable<WinLossColumnNameDto> columnNameData = new List<WinLossColumnNameDto>
            {
                CreateWinLossColumnNameDto(1, (int)BetType.Handicap),
                CreateWinLossColumnNameDto(1, (int)BetType.Over_Under),
                CreateWinLossColumnNameDto(1, (int)BetType.FT_1X2),
                CreateWinLossColumnNameDto(1, (int)BetType._1st_Handicap),
                CreateWinLossColumnNameDto(1, (int)BetType._1st_Over_Under),
                CreateWinLossColumnNameDto(1, (int)BetType.FirstHalf_1X2),
                CreateWinLossColumnNameDto(2, (int)BetType.Handicap),
                CreateWinLossColumnNameDto(2, (int)BetType.Over_Under),
                CreateWinLossColumnNameDto(2, (int)BetType.FT_1X2)
            };

            IEnumerable<MatchForecast> matchForecasts = new List<MatchForecast>
            {
                CreateMatchForecast(1),
                CreateMatchForecast(2)
            };

            _forecastDataAccess.GetMatchForecast(Arg.Any<ICriteria>()).Returns(Tuple.Create(matchForecasts, rawData, columnNameData));

            //// Act
            var matchForcasts = _forecastService.GetMatchForecasts(query);

            //// Assert
            var match1 = matchForcasts.ElementAt(0);
            var match2 = matchForcasts.ElementAt(1);
            Assert.True(
                match1.TotalTurnover == 600
                && match2.TotalTurnover == 300);
        }

        [Test]
        public void GetMatchForecast_CurrentTimeIsSecondHaft_RemoveFirstHaftForecast()
        {
            //// Arrange
            var query = new MatchForecastsQuery(1)
            {
                MatchIds = "1"
            };
            IEnumerable<BetTypeForecastDto> rawData = new List<BetTypeForecastDto>
            {
                CreateBetTypeForecastDto(1, (int)BetType.Handicap),
                CreateBetTypeForecastDto(1, (int)BetType.Over_Under),
                CreateBetTypeForecastDto(1, (int)BetType.FT_1X2),
                CreateBetTypeForecastDto(1, (int)BetType._1st_Handicap),
                CreateBetTypeForecastDto(1, (int)BetType._1st_Over_Under),
                CreateBetTypeForecastDto(1, (int)BetType.FirstHalf_1X2)
            };

            IEnumerable<WinLossColumnNameDto> columnNameData = new List<WinLossColumnNameDto>
            {
                CreateWinLossColumnNameDto(1, (int)BetType.Handicap),
                CreateWinLossColumnNameDto(1, (int)BetType.Over_Under),
                CreateWinLossColumnNameDto(1, (int)BetType.FT_1X2),
                CreateWinLossColumnNameDto(1, (int)BetType._1st_Handicap),
                CreateWinLossColumnNameDto(1, (int)BetType._1st_Over_Under),
                CreateWinLossColumnNameDto(1, (int)BetType.FirstHalf_1X2)
            };

            IEnumerable<MatchForecast> matchForecasts = new List<MatchForecast>
            {
                CreateMatchForecast(1, true, DateTime.Now.Subtract(new TimeSpan(1, 0, 0)))
            };

            _forecastDataAccess.GetMatchForecast(Arg.Any<ICriteria>()).Returns(Tuple.Create(matchForecasts, rawData, columnNameData));

            //// Act
            var matchForcasts = _forecastService.GetMatchForecasts(query);

            //// Assert
            Assert.True(matchForcasts.ElementAt(0).BetTypeForecasts.Count() == 3
                         && matchForcasts.ElementAt(0).BetTypeForecasts.ElementAt(0).BetTypeId == (int)BetType.Handicap
                         && matchForcasts.ElementAt(0).BetTypeForecasts.ElementAt(1).BetTypeId == (int)BetType.Over_Under
                         && matchForcasts.ElementAt(0).BetTypeForecasts.ElementAt(2).BetTypeId == (int)BetType.FT_1X2);
        }

        private BetTypeForecastDto CreateBetTypeForecastDto(
            int matchId = 1,
            int betTypeId = (int)BetType.Handicap,
            decimal firstWinLoss = 1m,
            decimal secondWinLoss = 2m,
            decimal thirdWinLoss = 3m,
            decimal fourthWinLoss = 4m,
            decimal fifthWinLoss = 5m)
        {
            return new BetTypeForecastDto
            {
                MatchId = matchId,
                BetTypeId = betTypeId,
                WinLossFirstColumn = firstWinLoss,
                WinLossSecondColumn = secondWinLoss,
                WinLossThirdColumn = thirdWinLoss,
                WinLossFourthColumn = fourthWinLoss,
                WinLossFifthColumn = fifthWinLoss
            };
        }

        private WinLossColumnNameDto CreateWinLossColumnNameDto(
            int matchId = 1,
            int betTypeId = (int)BetType.Handicap,
            int highlightValueOverUnder = 2,
            int firstWinLoss = 1,
            int secondWinLoss = 2,
            int thirdWinLoss = 3,
            int fourthWinLoss = 4,
            int fifthWinLoss = 5,
            decimal turnover = 100m,
            int betCount = 2)
        {
            return new WinLossColumnNameDto
            {
                MatchId = matchId,
                BetTypeId = betTypeId,
                FirstColumnName = firstWinLoss,
                SecondColumnName = secondWinLoss,
                ThirdColumnName = thirdWinLoss,
                FourthColumnName = fourthWinLoss,
                FifthColumnName = fifthWinLoss,
                HighlightValueOverUnder = highlightValueOverUnder,
                Turnover = turnover,
                BetCount = betCount
            };
        }

        private MatchForecast CreateMatchForecast(
            int matchId = 1,
            bool isLive = true,
            DateTime? kickOffTime = null,
            byte liveHomeScore = 0,
            byte liveAwayScore = 0)
        {
            return new MatchForecast
            {
                Id = matchId,
                IsLive = isLive,
                KickOffTime = kickOffTime.HasValue ? kickOffTime.Value : DateTime.Now.Add(new TimeSpan(1, 0, 0)),
                LiveHomeScore = liveHomeScore,
                LiveAwayScore = liveAwayScore
            };
        }
    }
}