namespace Alpha.TotalBetsForecast.Site.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Criteria;
    using DataAccesses;
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
        private ForecastService _forecastService;

        [SetUp]
        public void SetUp()
        {
            _forecastDataAccess = Substitute.For<IForecastDataAccess>();

            _forecastService = new ForecastService(_forecastDataAccess);
        }

        [Test]
        public void GetMatchForecast_QueryIsNull_ThrowArgumentNullException()
        {
            MatchForecastsQuery query = null;

            Assert.Throws<ArgumentNullException>(() => _forecastService.GetMatchForecasts(query));
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
            var query = new MatchForecastsQuery(1);
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
            var query = new MatchForecastsQuery(1);
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
        public void GetMatchForecast_MatchIsNoneLive_BuildWinLossNameListForOverUnder(BetType betType, int highLightValue, int highLightIndex)
        {
            //// Arrange
            var query = new MatchForecastsQuery(1);
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
                && matchForcasts.ElementAt(0).BetTypeForecasts.ElementAt(0).MainWinLossIndex == highLightIndex);
        }

        [TestCase(BetType.Over_Under)]
        [TestCase(BetType._1st_Over_Under)]
        public void GetMatchForecast_MatchIsLive_BuildWinLossNameListForOverUnder(BetType betType)
        {
            //// Arrange
            var query = new MatchForecastsQuery(1);
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

        [TestCase(BetType.FT_1X2)]
        [TestCase(BetType.FirstHalf_1X2)]
        public void GetMatchForecast_MatchIsNonLive_BuildWinLossNameListForOneXTwo(BetType betType)
        {
            //// Arrange
            var query = new MatchForecastsQuery(1);
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

        ////[Test]
        ////public void GetMatchForecast_CurrentTimeIsSecondHaft_RemoveFirstHaftForecast()
        ////{
        ////    //// Arrange
        ////    var query = new MatchForecastsQuery(1);
        ////    var kickOffTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
        ////    var rawData = new List<BetTypeForecastDto>
        ////    {
        ////        new BetTypeForecastDto { BetTypeId = (int)BetType.Handicap, BetChoice = "h" },
        ////        new BetTypeForecastDto { BetTypeId = (int)BetType._1st_Handicap, BetChoice = "a" },
        ////        new BetTypeForecastDto { BetTypeId = (int)BetType.Over_Under, BetChoice = "a" },
        ////        new BetTypeForecastDto { BetTypeId = (int)BetType._1st_Over_Under, BetChoice = "a" },
        ////        new BetTypeForecastDto { BetTypeId = (int)BetType.FT_1X2, BetChoice = "1" },
        ////        new BetTypeForecastDto { BetTypeId = (int)BetType.FirstHalf_1X2, BetChoice = "x" }
        ////    };

        ////    var columnNameData = new List<WinLossColumnNameDto>
        ////    {
        ////        new WinLossColumnNameDto { BetTypeId = (int)BetType.Handicap },
        ////        new WinLossColumnNameDto { BetTypeId = (int)BetType._1st_Handicap },
        ////        new WinLossColumnNameDto { BetTypeId = (int)BetType.Over_Under },
        ////        new WinLossColumnNameDto { BetTypeId = (int)BetType._1st_Over_Under },
        ////        new WinLossColumnNameDto { BetTypeId = (int)BetType.FT_1X2 },
        ////        new WinLossColumnNameDto { BetTypeId = (int)BetType.FirstHalf_1X2 },
        ////    };

        ////    var matchForecasts = new List<MatchForecast>
        ////    {
        ////        new MatchForecast
        ////        {
        ////            Id = 1
        ////        }
        ////    };

        ////    var forecastData = Tuple.Create(matchForecasts as IEnumerable<MatchForecast>, rawData as IEnumerable<BetTypeForecastDto>, columnNameData as IEnumerable<WinLossColumnNameDto>);


        ////    _forecastDataAccess.GetMatchForecast(Arg.Any<ICriteria>()).Returns(forecastData);

        ////    //// Act
        ////    var matchForcast = _forecastService.GetMatchForecasts(query);

        ////    //// Assert
        ////    ////Assert.True(matchForcast.BetTypeForecasts.Count() == 3
        ////    ////    && matchForcast.BetTypeForecasts.ElementAt(0).BetTypeId == (int)BetType.Handicap
        ////    ////    && matchForcast.BetTypeForecasts.ElementAt(0).BetChoices.Count() == 1
        ////    ////    && matchForcast.BetTypeForecasts.ElementAt(1).BetTypeId == (int)BetType.Over_Under
        ////    ////    && matchForcast.BetTypeForecasts.ElementAt(1).BetChoices.Count() == 1
        ////    ////    && matchForcast.BetTypeForecasts.ElementAt(2).BetTypeId == (int)BetType.FT_1X2
        ////    ////    && matchForcast.BetTypeForecasts.ElementAt(2).BetChoices.Count() == 1);
        ////}

        ////[TestCase(BetType.Handicap, "h", "1.5,2", "1.5,2")]
        ////[TestCase(BetType.Handicap, "h", "1.5,2", "1.5,2")]
        ////[TestCase(BetType._1st_Handicap, "a", "1.5,2", "1.5,2")]
        ////[TestCase(BetType._1st_Handicap, "a", "1.5,2", "1.5,2")]
        ////[TestCase(BetType.Over_Under, "h", "2", "2")]
        ////[TestCase(BetType.Over_Under, "a", "2", "u")]
        ////[TestCase(BetType._1st_Over_Under, "h", "2", "2")]
        ////[TestCase(BetType._1st_Over_Under, "a", "2", "u")]
        ////[TestCase(BetType.FT_1X2, "1", "", "1")]
        ////[TestCase(BetType.FT_1X2, "x", "", "x")]
        ////[TestCase(BetType.FT_1X2, "2", "", "2")]
        ////[TestCase(BetType.FirstHalf_1X2, "1", "", "1")]
        ////[TestCase(BetType.FirstHalf_1X2, "x", "", "x")]
        ////[TestCase(BetType.FirstHalf_1X2, "2", "", "2")]
        ////public void GetMatchForecast_ReceiveListOfBetTypeDto_SetHandicapValueForEachBetChoice(
        ////    BetType betType,
        ////    string betChoice,
        ////    string rawHandicapValue,
        ////    string expectedHandicapValue)
        ////{
        ////    //// Arrange
        ////    var query = new MatchForecastsQuery(1);
        ////    var kickOffTime = DateTime.Now.AddDays(1);
        ////    var rawData = new List<BetTypeForecastDto>
        ////    {
        ////        new BetTypeForecastDto
        ////        {
        ////            BetTypeId = (int)betType,
        ////            BetChoice = betChoice
        ////        }
        ////    };

        ////    var columnNameData = new List<WinLossColumnNameDto>
        ////    {
        ////        new WinLossColumnNameDto
        ////        {
        ////            BetTypeId = (int)betType
        ////        }
        ////    };

        ////    var matchForecasts = new List<MatchForecast>
        ////    {
        ////        new MatchForecast
        ////        {
        ////            Id = 1
        ////        }
        ////    };

        ////    var forecastData = Tuple.Create(matchForecasts as IEnumerable<MatchForecast>, rawData as IEnumerable<BetTypeForecastDto>, columnNameData as IEnumerable<WinLossColumnNameDto>);


        ////    _forecastDataAccess.GetMatchForecast(Arg.Any<ICriteria>()).Returns(forecastData);

        ////    //// Act
        ////    var matchForcast = _forecastService.GetMatchForecasts(query);

        ////    //// Assert
        ////    //Assert.True(matchForcast.BetTypeForecasts.Count() == 1
        ////    //    && matchForcast.BetTypeForecasts.First().BetChoices.First().HandicapValue == expectedHandicapValue);
        ////}

        ////[Test]
        ////public void GetMatchForecast_DataValid_ReturnMatchForecast()
        ////{
        ////    //// Arrange
        ////    var query = new MatchForecastsQuery(1);
        ////    var kickOffTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
        ////    var rawData = new List<BetTypeForecastDto>
        ////    {
        ////        new BetTypeForecastDto
        ////        {
        ////            BetTypeId = (int)BetType.Handicap,
        ////            BetChoice = "h"
        ////        },
        ////        new BetTypeForecastDto
        ////        {
        ////            BetTypeId = (int)BetType.Handicap,
        ////            BetChoice = "a"
        ////        }
        ////    };

        ////    var columnNameData = new List<WinLossColumnNameDto>
        ////    {
        ////        new WinLossColumnNameDto
        ////        {
        ////            BetTypeId = (int)BetType.Handicap,
        ////            FirstColumnName = 1,
        ////            SecondColumnName = 2,
        ////            ThirdColumnName = 3
        ////        }
        ////    };

        ////    var matchForecasts = new List<MatchForecast>
        ////    {
        ////        new MatchForecast
        ////        {
        ////            Id = 1
        ////        }
        ////    };

        ////    var forecastData = Tuple.Create(matchForecasts as IEnumerable<MatchForecast>, rawData as IEnumerable<BetTypeForecastDto>, columnNameData as IEnumerable<WinLossColumnNameDto>);


        ////    //// Act
        ////    var matchForcast = _forecastService.GetMatchForecasts(query);

        ////    //// Assert
        ////    //Assert.IsTrue(
        ////    //    1m == matchForcast.ForecastTotal
        ////    //    && matchForcast.BetTypeForecasts.Count() == 1);
        ////}

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
            int HighlightValueOverUnder = 2,
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
                HighlightValueOverUnder = HighlightValueOverUnder,
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