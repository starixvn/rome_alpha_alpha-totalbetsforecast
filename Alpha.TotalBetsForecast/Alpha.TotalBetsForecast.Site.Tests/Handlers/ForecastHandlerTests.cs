namespace Alpha.TotalBetsForecast.Site.Tests.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using App_GlobalResources;
    using Models;
    using Models.Requests;
    using NSubstitute;
    using NUnit.Framework;
    using Site.Handlers;
    using Site.Services;
    using Sunplus.Agent.Model.RemotingRef;

    [TestFixture]
    public class ForecastHandlerTests
    {
        private const int CustId = 12345;
        private IForecastService _forecastService;
        private IEventService _eventService;
        private ForecastHandler _forecastHandler;

        [SetUp]
        public void SetUp()
        {
            _forecastService = Substitute.For<IForecastService>();
            _eventService = Substitute.For<IEventService>();

            _forecastHandler = new ForecastHandler(_forecastService, _eventService);

            CacheRefFactory.CacheRef = Substitute.For<ICacheRefService>();
        }

        #region Query Index page

        [Test]
        public void HandleIndexQuery_QueryIsNull_ThrowArgumentNullException()
        {
            IndexQuery query = null;

            Assert.Throws<ArgumentNullException>(() => _forecastHandler.Handle(query));
        }

        [Test]
        public void HandleIndexQuery_ExecuteGetLeagues()
        {
            var query = new IndexQuery
            {
                IsLive = true
            };

            //// Act
            _forecastHandler.Handle(query);

            _eventService.Received().GetLeagues(true);
        }

        [Test]
        public void HandleIndexQuery_ReturnIndexViewModel()
        {
            //// Arrange
            var query = new IndexQuery
            {
                IsLive = true,
                LeagueId = 2
            };

            //// Act
            var indexViewModel = _forecastHandler.Handle(query);

            //// Assert
            Assert.IsTrue(indexViewModel.IsLive 
                && indexViewModel.LeagueId == query.LeagueId);
        }

        [Test]
        public void HandleIndexQuery_ReturnDealballLiveSelectItemList()
        {
            //// Arrange
            var query = new IndexQuery
            {
                IsLive = true,
                LeagueId = 2
            };

            //// Act
            var dealBallLiveValues = _forecastHandler.Handle(query).LiveValues;

            //// Assert
            Assert.IsTrue(
                dealBallLiveValues.ElementAt(0).Value == "true"
                && dealBallLiveValues.ElementAt(0).Text == Resources.MatchOfferLive
                && dealBallLiveValues.ElementAt(0).Selected
                && dealBallLiveValues.ElementAt(1).Value == "false"
                && dealBallLiveValues.ElementAt(1).Text == Resources.Deadball
                && !dealBallLiveValues.ElementAt(1).Selected);
        }

        [Test]
        public void HandleIndexQuery_ReturnSortedLeaguesByName()
        {
            //// Arrange
            var query = new IndexQuery
            {
                IsLive = true,
                LeagueId = 2
            };

            _eventService.GetLeagues(Arg.Any<bool>()).Returns(new List<League>
            {
                new League
                {
                    Id = 1
                },
                new League
                {
                    Id = 3
                }
            });

            CacheRefFactory.CacheRef.GetLeagueName(1).Returns("B");
            CacheRefFactory.CacheRef.GetLeagueName(3).Returns("A");

            //// Act
            var leagues = _forecastHandler.Handle(query).Leagues;

            //// Assert
            Assert.IsTrue(
                leagues.Count() == 2
                && leagues.ElementAt(0).Text == "A"
                && leagues.ElementAt(1).Text == "B");
        }

        #endregion Query Index page

        #region Query Dropdown list of match

        [Test]
        public void HandleMatchesQuery_QueryIsNull_ThrowArgumentNullException()
        {
            MatchesQuery query = null;

            Assert.Throws<ArgumentNullException>(() => _forecastHandler.Handle(query));
        }

        [Test]
        public void HandleMatchesQuery_ExecuteGetMatches()
        {
            //// Arrange
            var query = new MatchesQuery
            {
                MatchId = 1,
                LeagueId = 1,
                IsLive = true
            };

            //// Act
            _forecastHandler.Handle(query);

            //// Assert
            _eventService.Received().GetMatches(1, true);
        }

        [Test]
        public void HandleMatchesQuery_ReturnMatchesViewModel()
        {
            //// Arrnage
            var query = new MatchesQuery
            {
                MatchId = 1,
                LeagueId = 1,
                IsLive = true
            };
            _eventService.GetMatches(Arg.Any<int>(), Arg.Any<bool>()).Returns(new List<Match>());

            //// Act
            var viewModel = _forecastHandler.Handle(query);

            //// Assert
            Assert.IsTrue(
                viewModel.LeagueId == query.LeagueId
                && viewModel.MatchId == query.MatchId);
        }

        [Test]
        public void HandleMatchesQuery_ReturnMatchesOrderedByKickOffTimeDesc()
        {
            //// Arrnage
            var query = new MatchesQuery
            {
                MatchId = 1,
                LeagueId = 1,
                IsLive = true
            };
            _eventService.GetMatches(Arg.Any<int>(), Arg.Any<bool>()).Returns(new List<Match>
            {
                new Match
                {
                    Id = 1,
                    KickOffTime = new DateTime(1989, 5, 28)
                },
                new Match
                {
                    Id = 2,
                    KickOffTime = new DateTime(2000, 5, 28)
                }
            });

            //// Act
            var viewModel = _forecastHandler.Handle(query);

            //// Assert
            Assert.IsTrue(
                viewModel.Matches.ElementAt(0).Id == 2
                && viewModel.Matches.ElementAt(1).Id == 1);
        }

        #endregion Query Dropdown list of match

        #region Match Forecast Testing

        [Test]
        public void HandleMatchForecastQuery_QueryIsNull_ThrowArgumentNullException()
        {
            MatchForecastQuery query = null;

            Assert.Throws<ArgumentNullException>(() => _forecastHandler.Handle(query));
        }

        [Test]
        public void HandleMatchForecastQuery_ExecuteGetMatchInfo()
        {
            //// Arrange
            var query = new MatchForecastQuery(CustId)
            {
                MatchId = 1,
                IsLive = true
            };

            _eventService.GetMatchInfo(Arg.Any<bool>(), Arg.Any<int>()).Returns(new Match());

            //// Act
            _forecastHandler.Handle(query);

            //// Assert
            _eventService.Received().GetMatchInfo(query.IsLive, query.MatchId);
        }

        [Test]
        public void HandleMatchForecastQuery_MatchInfoIsNull_ThrowArgumentNullException()
        {
            //// Arrange
            var query = new MatchForecastQuery(CustId)
            {
                MatchId = 1,
                IsLive = true
            };
            Match match = null;

            _eventService.GetMatchInfo(Arg.Any<bool>(), Arg.Any<int>()).Returns(match);

            //// Assert
            Assert.Throws<ArgumentNullException>(() => _forecastHandler.Handle(query));
        }

        [Test]
        public void HandleMatchForecastQuery_ExecuteGetMatchForecast()
        {
            //// Arrange
            var query = new MatchForecastQuery(CustId)
            {
                MatchId = 1,
                IsLive = true
            };

            _eventService.GetMatchInfo(Arg.Any<bool>(), Arg.Any<int>()).Returns(new Match());

            //// Act
            _forecastHandler.Handle(query);

            //// Assert
            _forecastService.Received().GetMatchForecast(query);
        }

        [Test]
        public void HandleMatchForecastQuery_MatchHasData_UseMatchDataToQueryMatchForecast()
        {
            //// Arrange
            var query = new MatchForecastQuery(CustId)
            {
                MatchId = 1,
                IsLive = true
            };

            var returnMatch = new Match
            {
                Id = 1,
                LiveAwayScore = 3,
                LiveHomeScore = 4
            };

            _eventService.GetMatchInfo(Arg.Any<bool>(), Arg.Any<int>()).Returns(returnMatch);

            //// Act
            _forecastHandler.Handle(query);

            //// Assert
            _forecastService.Received().GetMatchForecast(
                Arg.Is<MatchForecastQuery>(actualQuery => 
                actualQuery.LiveAwayScore == returnMatch.LiveAwayScore
                && actualQuery.LiveHomeScore == returnMatch.LiveHomeScore));
        }

        [Test]
        public void HandleMatchForecastQuery_DataValid_ReturnMatchForecastViewMode()
        {
            //// Arrange
            var query = new MatchForecastQuery(CustId)
            {
                MatchId = 1,
                IsLive = true
            };

            Match match = new Match
            {
                Id = 1,
                AwayId = 1,
                Code = "aaaa",
                HasLive = true,
                HomeId = 3,
                IsLive = true,
                KickOffTime = new DateTime(1989, 5, 28),
                LeagueId = 1,
                LiveAwayScore = 3,
                LiveHomeScore = 4
            };

            _forecastService.GetMatchForecast(Arg.Any<MatchForecastQuery>()).Returns(new MatchForecast
            {
                ForecastTotal = 101,
                BetTypeForecasts = new List<BetTypeForecast>
                {
                    new BetTypeForecast
                    {
                        BetTypeId = 1,
                        BetType = "aaaaa"
                    }
                }
            });

            _eventService.GetMatchInfo(Arg.Any<bool>(), Arg.Any<int>()).Returns(match);

            //// Act
            var actualViewModel = _forecastHandler.Handle(query);

            //// Assert
            Assert.IsTrue(actualViewModel.MatchId == match.Id
                && actualViewModel.KickOffTimeString == "05/28/1989 12:00 AM"
                && actualViewModel.HasLive
                && actualViewModel.IsLive
                && actualViewModel.LiveAwayScore == match.LiveAwayScore
                && actualViewModel.LiveHomeScore == match.LiveHomeScore
                && actualViewModel.ForecastTotal == 101
                && actualViewModel.BetTypeForecasts.Count() == 1
                && actualViewModel.BetTypeForecasts.ElementAt(0).BetType == "aaaaa");
        }

        #endregion Match Forecast Testing
    }
}