namespace Alpha.TotalBetsForecast.Site.Tests.Models
{
    using Alpha.TotalBetsForecast.Site.Models;
    using NSubstitute;
    using NUnit.Framework;
    using RemotingRef;

    [TestFixture]
    public class MatchTests
    {
        [SetUp]
        public void Setup()
        {
            CacheRefFactory.CacheRef = Substitute.For<CacheRefService>();
        }

        [Test]
        public void Name_Always_ReturnExpectedText()
        {
            //// Arrange
            var home = new { Id = 1, Name = "MU" };
            var away = new { Id = 2, Name = "Liv" };
            var expectedText = $"{home.Name} - {away.Name}";
            var match = new Match { HomeId = home.Id, AwayId = away.Id };

            CacheRefFactory.CacheRef.GetTeamName(home.Id).Returns(home.Name);
            CacheRefFactory.CacheRef.GetTeamName(away.Id).Returns(away.Name);

            //// Act
            var act = match.Name;

            //// Assert
            Assert.AreEqual(expectedText, act);
        }
    }
}
