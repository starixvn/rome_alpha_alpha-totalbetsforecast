namespace Alpha.TotalBetsForecast.Site.Tests.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Models;
    using Models.Requests;
    using NSubstitute;
    using NUnit.Framework;
    using Services;
    using Site.Handlers;

    [TestFixture]
    public class ForecastHandlerTests
    {
        private IForecastService _forecastService;
        private IEventService _eventService;
        private ForecastHandler _forecastHandler;
        private const int CustId = 12345;

        [SetUp]
        public void SetUp()
        {
            _forecastService = Substitute.For<IForecastService>();
            _eventService = Substitute.For<IEventService>();

            _forecastHandler = new ForecastHandler(_forecastService, _eventService);
        }

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

            _eventService.GetMatchInfo(Arg.Any<bool>(), Arg.Any<int>()).Returns(match);

            //// Act
            var actualViewModel = _forecastHandler.Handle(query);


        }
    }
}
