namespace Alpha.TotalBetsForecast.Site.Tests.Handlers
{
    using Alpha.TotalBetsForecast.Models;
    using Alpha.TotalBetsForecast.Services;
    using Alpha.TotalBetsForecast.Site.Handlers;
    using Alpha.TotalBetsForecast.Site.Requests;
    using NSubstitute;
    using NUnit.Framework;
    using Ploeh.AutoFixture;
    using Sunplus.Agent.Model.Entities;

    [TestFixture]
    public class BullfightingHandlerTests
    {
        private BullfightingService service;
        private IUserSession session;
        private BullfightingHandler handler;
        private Fixture fixture;

        [SetUp]
        public void Setup()
        {
            service = Substitute.For<BullfightingService>();
            handler = new BullfightingHandler(service);
            fixture = new Fixture();
            session = Substitute.For<IUserSession>();
            session.CustId.Returns(12);
            session.RoleId.Returns((byte)4);
        }

        [Test]
        public void Handle_Always_CallService()
        {
            //// Arrange
            service.GetMatchs(Arg.Any<int>(), Arg.Any<UserLevel>()).Returns(fixture.CreateMany<BullfightingModel>());
            var query = new BullfightingQuery(session);

            //// Act
            handler.Handle(query);

            //// Assert
            service.Received().GetMatchs(Arg.Any<int>(), Arg.Any<UserLevel>());
        }
    }
}
