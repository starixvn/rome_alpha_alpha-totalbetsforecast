////namespace Alpha.TotalBetsForecast.Site.Tests.Services
////{
////    using System;
////    using System.Collections.Generic;
////    using System.Linq;
////    using Criteria;
////    using DataAccesses;
////    using Fanex.Data.Repository;
////    using Models;
////    using Models.Dtos;
////    using Models.Requests;
////    using NSubstitute;
////    using NUnit.Framework;
////    using Site.Services.Impl;
////    using Sunplus.Agent.Model.BL.Definitions;

////    [TestFixture]
////    public class ForecastServiceTests
////    {
////        private IForecastDataAccess _forecastDataAccess;
////        private ForecastService _forecastService;

////        [SetUp]
////        public void SetUp()
////        {
////            _forecastDataAccess = Substitute.For<IForecastDataAccess>();

////            _forecastService = new ForecastService(_forecastDataAccess);
////        }

////        [Test]
////        public void GetMatchForecast_QueryIsNull_ThrowArgumentNullException()
////        {
////            MatchForecastsQuery query = null;

////            Assert.Throws<ArgumentNullException>(() => _forecastService.GetMatchForecasts(query));
////        }

////        [Test]
////        public void GetMatchForecast_ExecuteGetMatchForecast()
////        {
////            //// Arrange
////            var query = new MatchForecastsQuery(1)
////            {
////                IsLive = true,
////                LeagueId = 1,
////                MatchIds = "1,2"
////            };

////            var forecastData = Tuple.Create(Enumerable.Empty<MatchForecast>(), Enumerable.Empty<BetTypeForecastDto>(), Enumerable.Empty<WinLossColumnNameDto>());

////            _forecastDataAccess.GetMatchForecast(Arg.Any<ICriteria>()).Returns(forecastData);

////            //// Act
////            _forecastService.GetMatchForecasts(query);

////            //// Assert
////            _forecastDataAccess.Received().GetMatchForecast(
////                Arg.Is<GetMatchForecastsCriteria>(
////                    criteria => criteria.MatchIds == query.MatchIds
////                            && criteria.IsLive
////                            && criteria.CustId == 1));
////        }

////        [Test]
////        public void GetMatchForecast_ReceiveListOfBetTypeDto_GroupDataByBetType()
////        {
////            //// Arrange
////            var query = new MatchForecastsQuery(1);
////            var kickOffTime = DateTime.Now.AddDays(1);
////            var rawData = new List<BetTypeForecastDto>
////            {
////                new BetTypeForecastDto { MatchId = 1, BetTypeId = (int)BetType.Handicap },
////                new BetTypeForecastDto { MatchId = 1, BetTypeId = (int)BetType.Handicap },
////                new BetTypeForecastDto { MatchId = 1, BetTypeId = (int)BetType._1st_Over_Under },
////                new BetTypeForecastDto { MatchId = 1, BetTypeId = (int)BetType.FT_1X2 },
////                new BetTypeForecastDto { MatchId = 1, BetTypeId = (int)BetType.FT_1X2 }
////            };

////            var columnNameData = new List<WinLossColumnNameDto>
////            {
////                new WinLossColumnNameDto
////                {
////                    MatchId = 1,
////                    BetTypeId = (int)BetType.Handicap
////                },
////                new WinLossColumnNameDto
////                {
////                    MatchId = 1,
////                    BetTypeId = (int)BetType._1st_Over_Under
////                }
////            };


////            var forecastData = Tuple.Create(rawData as IEnumerable<BetTypeForecastDto>, columnNameData as IEnumerable<WinLossColumnNameDto>);

////            _forecastDataAccess.GetMatchForecast(Arg.Any<ICriteria>()).Returns(forecastData);

////            //// Act
////            var matchForcasts = _forecastService.GetMatchForecasts(query);

////            //// Assert
////            Assert.True(matchForcasts.BetTypeForecasts.Count() == 3
////                && matchForcasts.BetTypeForecasts.ElementAt(0).BetTypeId == (int)BetType.Handicap
////                && matchForcasts.BetTypeForecasts.ElementAt(0).BetChoices.Count() == 2
////                && matchForcasts.BetTypeForecasts.ElementAt(1).BetTypeId == (int)BetType._1st_Over_Under
////                && matchForcasts.BetTypeForecasts.ElementAt(1).BetChoices.Count() == 1
////                && matchForcasts.BetTypeForecasts.ElementAt(2).BetTypeId == (int)BetType.FT_1X2
////                && matchForcasts.BetTypeForecasts.ElementAt(2).BetChoices.Count() == 2);
////        }

////        [TestCase(BetType.Handicap)]
////        [TestCase(BetType._1st_Handicap)]
////        [TestCase(BetType.Over_Under)]
////        [TestCase(BetType._1st_Over_Under)]
////        public void GetMatchForecast_BetTypeIsHandicapOrOverUnder_AddColumnNameToFirstBetChoice(BetType betType)
////        {
////            //// Arrange
////            var query = new MatchForecastsQuery(1);
////            var kickOffTime = DateTime.Now.AddDays(1);
////            var rawData = new List<BetTypeForecastDto>
////            {
////                new BetTypeForecastDto
////                {
////                    BetTypeId = (int)betType,
////                    BetChoice = "h"
////                },
////                new BetTypeForecastDto
////                {
////                    BetTypeId = (int)betType,
////                    BetChoice = "a"
////                }
////            };

////            var columnNameData = new List<WinLossColumnNameDto>
////            {
////                new WinLossColumnNameDto
////                {
////                    BetTypeId = (int)betType,
////                    FirstColumnName = "FirstColumnName",
////                    SecondColumnName = "SecondColumnName",
////                    ThirdColumnName = "ThirdColumnName"
////                }
////            };

////            var forecastData = Tuple.Create(rawData as IEnumerable<BetTypeForecastDto>, columnNameData as IEnumerable<WinLossColumnNameDto>, 1m);

////            _forecastDataAccess.GetMatchForecast(Arg.Any<ICriteria>()).Returns(forecastData);

////            //// Act
////            var matchForcast = _forecastService.GetMatchForecasts(query, kickOffTime);

////            //// Assert
////            Assert.True(
////                matchForcast.BetTypeForecasts.ElementAt(0).BetChoices.First().WinLossColumnNames.Count() == 3
////                && matchForcast.BetTypeForecasts.ElementAt(0).BetChoices.First().WinLossColumnNames.First() == columnNameData.First().FirstColumnName
////                && matchForcast.BetTypeForecasts.ElementAt(0).BetChoices.First().WinLossColumnNames.ElementAt(1) == columnNameData.First().SecondColumnName
////                && matchForcast.BetTypeForecasts.ElementAt(0).BetChoices.First().WinLossColumnNames.ElementAt(2) == columnNameData.First().ThirdColumnName
////                && !matchForcast.BetTypeForecasts.ElementAt(0).BetChoices.ElementAt(1).WinLossColumnNames.Any());
////        }

////        [TestCase(BetType.FT_1X2)]
////        [TestCase(BetType.FirstHalf_1X2)]
////        public void GetMatchForecast_BetTypeIsOneXTwo_AddEmptyColumnNameToFirstBetChoice(BetType betType)
////        {
////            //// Arrange
////            var query = new MatchForecastsQuery(1);
////            var kickOffTime = DateTime.Now.AddDays(1);
////            var rawData = new List<BetTypeForecastDto>
////            {
////                new BetTypeForecastDto
////                {
////                    BetTypeId = (int)betType,
////                    BetChoice = "h"
////                }
////            };

////            var columnNameData = new List<WinLossColumnNameDto>
////            {
////                new WinLossColumnNameDto
////                {
////                    BetTypeId = (int)betType,
////                    FirstColumnName = string.Empty
////                }
////            };

////            var forecastData = Tuple.Create(rawData as IEnumerable<BetTypeForecastDto>, columnNameData as IEnumerable<WinLossColumnNameDto>, 1m);

////            _forecastDataAccess.GetMatchForecast(Arg.Any<ICriteria>()).Returns(forecastData);

////            //// Act
////            var matchForcast = _forecastService.GetMatchForecasts(query, kickOffTime);

////            //// Assert
////            Assert.IsEmpty(matchForcast.BetTypeForecasts.ElementAt(0).BetChoices.First().WinLossColumnNames);
////        }

////        [Test]
////        public void GetMatchForecast_CurrentTimeIsSecondHaft_RemoveFirstHaftForecast()
////        {
////            //// Arrange
////            var query = new MatchForecastsQuery(1);
////            var kickOffTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
////            var rawData = new List<BetTypeForecastDto>
////            {
////                new BetTypeForecastDto { BetTypeId = (int)BetType.Handicap, BetChoice = "h" },
////                new BetTypeForecastDto { BetTypeId = (int)BetType._1st_Handicap, BetChoice = "a" },
////                new BetTypeForecastDto { BetTypeId = (int)BetType.Over_Under, BetChoice = "a" },
////                new BetTypeForecastDto { BetTypeId = (int)BetType._1st_Over_Under, BetChoice = "a" },
////                new BetTypeForecastDto { BetTypeId = (int)BetType.FT_1X2, BetChoice = "1" },
////                new BetTypeForecastDto { BetTypeId = (int)BetType.FirstHalf_1X2, BetChoice = "x" }
////            };

////            var columnNameData = new List<WinLossColumnNameDto>
////            {
////                new WinLossColumnNameDto { BetTypeId = (int)BetType.Handicap },
////                new WinLossColumnNameDto { BetTypeId = (int)BetType._1st_Handicap },
////                new WinLossColumnNameDto { BetTypeId = (int)BetType.Over_Under },
////                new WinLossColumnNameDto { BetTypeId = (int)BetType._1st_Over_Under },
////                new WinLossColumnNameDto { BetTypeId = (int)BetType.FT_1X2 },
////                new WinLossColumnNameDto { BetTypeId = (int)BetType.FirstHalf_1X2 },
////            };

////            var forecastData = Tuple.Create(rawData as IEnumerable<BetTypeForecastDto>, columnNameData as IEnumerable<WinLossColumnNameDto>, 1m);

////            _forecastDataAccess.GetMatchForecast(Arg.Any<ICriteria>()).Returns(forecastData);

////            //// Act
////            var matchForcast = _forecastService.GetMatchForecasts(query, kickOffTime);

////            //// Assert
////            Assert.True(matchForcast.BetTypeForecasts.Count() == 3
////                && matchForcast.BetTypeForecasts.ElementAt(0).BetTypeId == (int)BetType.Handicap
////                && matchForcast.BetTypeForecasts.ElementAt(0).BetChoices.Count() == 1
////                && matchForcast.BetTypeForecasts.ElementAt(1).BetTypeId == (int)BetType.Over_Under
////                && matchForcast.BetTypeForecasts.ElementAt(1).BetChoices.Count() == 1
////                && matchForcast.BetTypeForecasts.ElementAt(2).BetTypeId == (int)BetType.FT_1X2
////                && matchForcast.BetTypeForecasts.ElementAt(2).BetChoices.Count() == 1);
////        }

////        [TestCase(BetType.Handicap, "h", "1.5,2", "1.5,2")]
////        [TestCase(BetType.Handicap, "h", "1.5,2", "1.5,2")]
////        [TestCase(BetType._1st_Handicap, "a", "1.5,2", "1.5,2")]
////        [TestCase(BetType._1st_Handicap, "a", "1.5,2", "1.5,2")]
////        [TestCase(BetType.Over_Under, "h", "2", "2")]
////        [TestCase(BetType.Over_Under, "a", "2", "u")]
////        [TestCase(BetType._1st_Over_Under, "h", "2", "2")]
////        [TestCase(BetType._1st_Over_Under, "a", "2", "u")]
////        [TestCase(BetType.FT_1X2, "1", "", "1")]
////        [TestCase(BetType.FT_1X2, "x", "", "x")]
////        [TestCase(BetType.FT_1X2, "2", "", "2")]
////        [TestCase(BetType.FirstHalf_1X2, "1", "", "1")]
////        [TestCase(BetType.FirstHalf_1X2, "x", "", "x")]
////        [TestCase(BetType.FirstHalf_1X2, "2", "", "2")]
////        public void GetMatchForecast_ReceiveListOfBetTypeDto_SetHandicapValueForEachBetChoice(
////            BetType betType,
////            string betChoice,
////            string rawHandicapValue,
////            string expectedHandicapValue)
////        {
////            //// Arrange
////            var query = new MatchForecastsQuery(1);
////            var kickOffTime = DateTime.Now.AddDays(1);
////            var rawData = new List<BetTypeForecastDto>
////            {
////                new BetTypeForecastDto
////                {
////                    BetTypeId = (int)betType,
////                    BetType = rawHandicapValue,
////                    BetChoice = betChoice
////                }
////            };

////            var columnNameData = new List<WinLossColumnNameDto>
////            {
////                new WinLossColumnNameDto
////                {
////                    BetTypeId = (int)betType
////                }
////            };

////            var forecastData = Tuple.Create(rawData as IEnumerable<BetTypeForecastDto>, columnNameData as IEnumerable<WinLossColumnNameDto>, 1m);

////            _forecastDataAccess.GetMatchForecast(Arg.Any<ICriteria>()).Returns(forecastData);

////            //// Act
////            var matchForcast = _forecastService.GetMatchForecasts(query, kickOffTime);

////            //// Assert
////            Assert.True(matchForcast.BetTypeForecasts.Count() == 1
////                && matchForcast.BetTypeForecasts.First().BetChoices.First().HandicapValue == expectedHandicapValue);
////        }

////        [Test]
////        public void GetMatchForecast_DataValid_ReturnMatchForecast()
////        {
////            //// Arrange
////            var query = new MatchForecastsQuery(1);
////            var kickOffTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0));
////            var rawData = new List<BetTypeForecastDto>
////            {
////                new BetTypeForecastDto
////                {
////                    BetTypeId = (int)BetType.Handicap,
////                    BetChoice = "h"
////                },
////                new BetTypeForecastDto
////                {
////                    BetTypeId = (int)BetType.Handicap,
////                    BetChoice = "a"
////                }
////            };

////            var columnNameData = new List<WinLossColumnNameDto>
////            {
////                new WinLossColumnNameDto
////                {
////                    BetTypeId = (int)BetType.Handicap,
////                    FirstColumnName = "FirstColumnName",
////                    SecondColumnName = "SecondColumnName",
////                    ThirdColumnName = "ThirdColumnName"
////                }
////            };

////            var forecastData = Tuple.Create(rawData as IEnumerable<BetTypeForecastDto>, columnNameData as IEnumerable<WinLossColumnNameDto>, 1m);

////            _forecastDataAccess.GetMatchForecast(Arg.Any<ICriteria>()).Returns(forecastData);

////            //// Act
////            var matchForcast = _forecastService.GetMatchForecasts(query, kickOffTime);

////            //// Assert
////            Assert.IsTrue(
////                1m == matchForcast.ForecastTotal
////                && matchForcast.BetTypeForecasts.Count() == 1);
////        }
////    }
////}