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
    using Sunplus.Agent.Model.BL.Definitions;
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
        public void HandleLeaguesQuery_QueryIsNull_ThrowArgumentNullException()
        {
            LeaguesQuery query = null;

            Assert.Throws<ArgumentNullException>(() => _forecastHandler.Handle(query));
        }

        [Test]
        public void HandleLeaguesQuery_ExecuteGetLeagues()
        {
            var query = new LeaguesQuery
            {
                IsLive = true
            };

            //// Act
            _forecastHandler.Handle(query);

            _eventService.Received().GetLeagues(true);
        }

        [Test]
        public void HandleLeaguesQuery_ReturnSortedLeaguesByName()
        {
            //// Arrange
            var query = new LeaguesQuery
            {
                IsLive = true
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
            var leagues = _forecastHandler.Handle(query);

            //// Assert
            Assert.IsTrue(
                leagues.Count() == 2
                && leagues.ElementAt(0).Name == "A"
                && leagues.ElementAt(1).Name == "B");
        }

        [Test]
        public void HandleIndexQuery_QueryIsNull_ThrowArgumentNullException()
        {
            IndexQuery query = null;

            Assert.Throws<ArgumentNullException>(() => _forecastHandler.Handle(query));
        }

        [Test]
        public void HandleIndexQuery_ReturnIndexViewModel()
        {
            //// Arrange
            var query = new IndexQuery
            {
                IsLive = true
            };

            //// Act
            var indexViewModel = _forecastHandler.Handle(query);

            //// Assert
            Assert.IsTrue(indexViewModel.IsLive);
        }

        [Test]
        public void HandleIndexQuery_ReturnDealballLiveSelectItemList()
        {
            //// Arrange
            var query = new IndexQuery
            {
                IsLive = true
            };

            //// Act
            var dealBallLiveValues = _forecastHandler.Handle(query).LiveValues;

            //// Assert
            Assert.IsTrue(
                dealBallLiveValues.ElementAt(0).Value == "true"
                && dealBallLiveValues.ElementAt(0).Text == Resources.MatchOfferLive
                && dealBallLiveValues.ElementAt(0).Selected
                && dealBallLiveValues.ElementAt(1).Value == "false"
                && dealBallLiveValues.ElementAt(1).Text == Resources.NonLive
                && !dealBallLiveValues.ElementAt(1).Selected);
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
                LeagueId = 1,
                IsLive = true
            };

            //// Act
            _forecastHandler.Handle(query);

            //// Assert
            _eventService.Received().GetMatches(1, true);
        }

        [Test]
        public void HandleMatchesQuery_ReturnMatchesOrderedByKickOffTimeDesc()
        {
            //// Arrnage
            var query = new MatchesQuery
            {
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
                viewModel.ElementAt(0).Id == 2
                && viewModel.ElementAt(1).Id == 1);
        }

        #endregion Query Dropdown list of match

        #region Match Forecast Testing

        [Test]
        public void HandleMatchForecastQuery_QueryIsNull_ThrowArgumentNullException()
        {
            MatchForecastsQuery query = null;

            Assert.Throws<ArgumentNullException>(() => _forecastHandler.Handle(query));
        }

        [Test]
        public void HandleMatchForecastQuery_ExecuteGetMatchInfo()
        {
            //// Arrange
            var query = new MatchForecastsQuery(CustId)
            {
                MatchIds = "1",
                IsLive = true
            };

            //// Act
            _forecastHandler.Handle(query);

            //// Assert
            _forecastService.Received().GetMatchForecasts(query);
        }

        [Test]
        public void HandleMatchForecastQuery_DataValid_ReturnMatchForecastViewMode()
        {
            //// Arrange
            var query = new MatchForecastsQuery(CustId)
            {
                MatchIds = "1",
                IsLive = true
            };

            var match = new MatchForecast
            {
                Id = 1,
                AwayId = 1,
                HasLive = true,
                HomeId = 3,
                IsLive = true,
                KickOffTime = new DateTime(1989, 5, 28),
                LiveAwayScore = 3,
                LiveHomeScore = 4,
                ShowTime = "Show Time",
                TotalBetCount = 4,
                TotalForecastWinLoss = 5.1m,
                TotalTurnover = 3.4m,
                BetTypeForecasts = new List<BetTypeForecast>
                {
                    new BetTypeForecast { BetTypeId = (int)BetType.Handicap }
                }
            };

            _forecastService.GetMatchForecasts(Arg.Any<MatchForecastsQuery>()).Returns(new List<MatchForecast>
            {
                match
            });

            //// Act
            var actualViewModel = _forecastHandler.Handle(query);

            //// Assert

            Assert.IsTrue(actualViewModel.ElementAt(0).Id == match.Id
                && actualViewModel.ElementAt(0).HasLive
                && actualViewModel.ElementAt(0).IsLive
                && actualViewModel.ElementAt(0).LiveAwayScore == match.LiveAwayScore
                && actualViewModel.ElementAt(0).LiveHomeScore == match.LiveHomeScore
                && actualViewModel.ElementAt(0).ShowTime == "Show<br />Time"
                && actualViewModel.ElementAt(0).TotalBetCount == match.TotalBetCount
                && actualViewModel.ElementAt(0).TotalForecastWinLoss == match.TotalForecastWinLoss
                && actualViewModel.ElementAt(0).TotalTurnover == match.TotalTurnover
                && actualViewModel.ElementAt(0).BetTypeForecasts.Count() == 1
                && actualViewModel.ElementAt(0).BetTypeForecasts.ElementAt(0).BetTypeId == (int)BetType.Handicap);
        }

        #endregion Match Forecast Testing
    }
}