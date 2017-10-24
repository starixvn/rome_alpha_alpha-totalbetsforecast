namespace Alpha.TotalBetsForecast.Site.Tests.Models.Requests
{
    using Alpha.TotalBetsForecast.Site.Models.Requests;
    using NSubstitute;
    using NUnit.Framework;
    using Sunplus.Agent.Model.Entities;

    [TestFixture]
    public class MatchForecastsQueryTests
    {
        [Test]
        public void CustId_Always_EqualWithUserSession()
        {
            //// Arrange
            var custId = 123;
            var session = Substitute.For<IUserSession>();
            session.CustId.Returns(custId);
            var query = new MatchForecastsQuery(session);

            //// Act
            var act = query.CustId;

            //// Assert
            Assert.AreEqual(custId, act);
        }
    }
}
