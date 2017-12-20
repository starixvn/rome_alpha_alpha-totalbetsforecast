namespace Alpha.TotalBetsForecast.Tests.Services
{
    using Alpha.TotalBetsForecast.Criteria;
    using Alpha.TotalBetsForecast.Models;
    using Alpha.TotalBetsForecast.Services;
    using Fanex.Data.Repository;
    using NSubstitute;
    using NUnit.Framework;
    using Ploeh.AutoFixture;

    [TestFixture]
    public class BullfightingServiceTests
    {
        private IDynamicRepository repository;
        private BullfightingService service;
        private Fixture fixture;
        [SetUp]
        public void Setup()
        {
            repository = Substitute.For<IDynamicRepository>();
            service = new BullfightingServiceImpl(repository);
            fixture = new Fixture();
        }

        [Test]
        public void GetMatchs_Always_CallRepository()
        {
            //// Arrange
            repository.Fetch<BullfightingModel>(Arg.Any<GetBullfightingMatchCriteria>()).Returns(fixture.CreateMany<BullfightingModel>());

            //// Act
            service.GetMatchs(123, UserLevel.Agent);

            //// Assert
            repository.Received().Fetch<BullfightingModel>(Arg.Any<GetBullfightingMatchCriteria>());
        }
    }
}
