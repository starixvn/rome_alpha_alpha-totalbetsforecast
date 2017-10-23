namespace Alpha.TotalBetsForecast.Site.Tests.Controllers
{
    using Alpha.TotalBetsForecast.Site.Controllers;
    using Alpha.TotalBetsForecast.Site.Models.Requests;
    using MediatR;
    using NSubstitute;
    using NUnit.Framework;
    using Sunplus.Agent.Model.Entities;

    public class ForecastControllerTests
    {
        private IMediator mediator;
        private ForecastController controller;
        private IUserSession user;

        [SetUp]
        public void SetUp()
        {
            mediator = Substitute.For<IMediator>();
            controller = new ForecastController(mediator);
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
            var query = new MatchesQuery(123, true, true);

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
    }
}
