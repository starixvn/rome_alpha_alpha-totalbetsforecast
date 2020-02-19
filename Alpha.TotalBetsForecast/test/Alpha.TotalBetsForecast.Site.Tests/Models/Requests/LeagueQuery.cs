namespace Alpha.TotalBetsForecast.Site.Tests.Models.Requests
{
    using System;
    using Alpha.TotalBetsForecast.Site.Models.Requests;
    using NUnit.Framework;

    [TestFixture]
    public class LeagueQueryTests
    {
        [Test]
        public void DefaultConstructor_Always_DoNotEveryThing()
        {
            //// Arrange
            var query = new LeaguesQuery(false, false);

            //// Act            

            //// Assert
            Assert.IsFalse(query.IsLive);
            Assert.IsFalse(query.IsLocalTest);
        }

        [Test]
        public void Constructor_Always_ThrowException()
        {
            //// Arrange            

            //// Act            

            //// Assert
            Assert.Throws<ArgumentNullException>(() => new LeaguesQuery(true));
        }
    }
}
