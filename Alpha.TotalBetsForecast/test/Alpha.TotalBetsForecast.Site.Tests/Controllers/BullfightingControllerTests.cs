namespace Alpha.TotalBetsForecast.Site.Tests.Controllers
{
    using Alpha.TotalBetsForecast.Site.Controllers;
    using Alpha.TotalBetsForecast.Site.Requests;
    using MediatR;
    using NSubstitute;
    using NUnit.Framework;
    using Sunplus.Agent.Model.Entities;

    public class BullfightingControllerTests
    {
        private IMediator mediator;
        private BullfightingController controller;
        private IUserSession user;

        [SetUp]
        public void SetUp()
        {
            mediator = Substitute.For<IMediator>();
            controller = new BullfightingController(mediator);
            user = Substitute.For<IUserSession>();
            user.CustId.Returns(123);
        }

        [Test]
        public void GetMatches_Always_CallMediatorSend()
        {
            //// Arrange
            var query = new BullfightingQuery(user);

            //// Act
            controller.GetMatches(query);

            //// Assert
            mediator.Received().Send(query);
        }
    }
}
