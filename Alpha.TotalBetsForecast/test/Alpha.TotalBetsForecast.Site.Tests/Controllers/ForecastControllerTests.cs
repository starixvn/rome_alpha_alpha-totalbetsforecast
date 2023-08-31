namespace Alpha.TotalBetsForecast.Site.Tests.Controllers
{
    using System.Collections.Specialized;
    using Alpha.TotalBetsForecast.Site.Controllers;
    using Alpha.TotalBetsForecast.Site.Models.Requests;
    using AlphaPlus.User.Models;
    using MediatR;
    using NSubstitute;
    using NUnit.Framework;
    using Site.App_Start;

    [TestFixture]
    public class ForecastControllerTests
    {
        private IMediator mediator;
        private ForecastController controller;
        private IUserSession user;

        [SetUp]
        public void SetUp()
        {
            mediator = Substitute.For<IMediator>();
            controller = new ForecastController(mediator, user);
            user = Substitute.For<IUserSession>();
            user.CustId.Returns(123);
        }

        [Test]
        public void Index_Always_CallMediatorSend()
        {
            //// Arrange
            var query = new IndexQuery();

            //// Act
            controller.Index(query);

            //// Assert
            mediator.Received().Send(query);
        }

        [Test]
        public void GetMatches_Always_CallMediatorSend()
        {
            //// Arrange
            var query = new MatchesQuery { LeagueId = 123, IsLive = true };

            //// Act
            controller.GetMatches(query);

            //// Assert
            mediator.Received().Send(query);
        }

        [Test]
        public void GetMatchForecasts_Always_CallMediatorSend()
        {
            //// Arrange
            var query = new MatchForecastsQuery(user);

            //// Act
            controller.GetMatchForecasts(query);

            //// Assert
            mediator.Received().Send(query);
        }

        [Test]
        public void GetLeagues_Always_CallMediatorSend()
        {
            //// Arrange
            var query = new LeaguesQuery(true, true);

            //// Act
            controller.GetLeagues(query);

            //// Assert
            mediator.Received().Send(query);
        }

        [Test]
        public void NotAllowSiteToViewForecast_ContainSite_ReturnFalse()
        {
            //// Arrange
            var site = "abc";
            AppConfig.AppSettings = new NameValueCollection { ["NewTBF:AllowSitesToViewForecast"] = site };
            user.Site.Returns(site);

            //// Act
            var act = controller.NotAllowSiteToViewForecast();

            //// Assert
            Assert.IsFalse(act);
        }

        [Test]
        public void NotAllowSiteToViewForecast_NullSite_ReturnFalse()
        {
            //// Arrange
            var site = string.Empty;
            AppConfig.AppSettings = new NameValueCollection { ["NewTBF:AllowSitesToViewForecast"] = site };
            user.Site.Returns(site);

            //// Act
            var act = controller.NotAllowSiteToViewForecast();

            //// Assert
            Assert.IsFalse(act);
        }

        [Test]
        public void NotAllowSiteToViewForecast_NotContain_ReturnTrue()
        {
            //// Arrange            
            AppConfig.AppSettings = new NameValueCollection { ["NewTBF:AllowSitesToViewForecast"] = "site1" };
            user.Site.Returns("site2");

            //// Act
            var act = controller.NotAllowSiteToViewForecast();

            //// Assert
            Assert.IsTrue(act);
        }
    }
}
