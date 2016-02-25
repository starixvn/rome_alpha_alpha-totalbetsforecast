namespace Alpha.TotalBetsForecast.Site.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Criteria;
    using DataAccesses;
    using Models.Requests;
    using NSubstitute;
    using NUnit.Framework;
    using Site.Services.Impl;

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
            MatchForecastQuery query = null;

            Assert.Throws<ArgumentNullException>(() => _forecastService.GetMatchForecast(query));
        }

        [Test]
        public void GetMatchForecast_ExecuteGetMatchForecast()
        {
            var query = new MatchForecastQuery(1)
            {
                IsLive = true,
                LiveHomeScore = 2,
                LiveAwayScore = 1,
                MatchId = 1
            };

            _forecastService.GetMatchForecast(query);

            _forecastDataAccess.Received().GetMatchForecast(Arg.Is<GetMatchForecastCriteria>(
                    criteria => criteria.MatchId == query.MatchId
                            && criteria.LiveHomeScore == query.LiveHomeScore
                            && criteria.LiveAwayScore == query.LiveAwayScore
                            && criteria.IsLive
                ));
        }
    }
}
