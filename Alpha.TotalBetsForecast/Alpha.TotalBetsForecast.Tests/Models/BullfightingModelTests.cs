namespace Alpha.TotalBetsForecast.Tests.Models
{
    using System;
    using Alpha.TotalBetsForecast.Models;
    using NSubstitute;
    using NUnit.Framework;
    using Sunplus.Agent.Model.RemotingRef;

    [TestFixture]
    public class BullfightingModelTests
    {
        private BullfightingModel model;

        [SetUp]
        public void Setup()
        {
            model = new BullfightingModel(new DateTime(2017, month: 10, day: 24), 1, 2, true, 1, 2, 3, turnover: 100, banker: 29, player: 39);
            CacheRefFactory.CacheRef = Substitute.For<ICacheRefService>();
        }

        [Test]
        public void EventDateToString_Always_CorrectFormat()
        {
            //// Arrange
            var expectedFormat = "10/24";

            //// Act
            var act = model.EventDateToString;

            //// Assert
            Assert.AreEqual(expectedFormat, act);
        }

        [Test]
        public void LeagueName_Always_EqualInRemotingRef()
        {
            //// Arrange
            var remotingRef = "leagueabc";
            CacheRefFactory.CacheRef.GetLeagueName(model.LeagueId).Returns(remotingRef);

            //// Act
            var act = model.LeagueName;

            //// Assert
            Assert.AreEqual(remotingRef, act);
        }

        [Test]
        public void HomeName_Always_EqualInRemotingRef()
        {
            //// Arrange
            var remotingRef = "homeabc";
            CacheRefFactory.CacheRef.GetTeamName(model.HomeId).Returns(remotingRef);

            //// Act
            var act = model.HomeName;

            //// Assert
            Assert.AreEqual(remotingRef, act);
        }

        [Test]
        public void AwayName_Always_EqualInRemotingRef()
        {
            //// Arrange
            var remotingRef = "awayabc";
            CacheRefFactory.CacheRef.GetTeamName(model.AwayId).Returns(remotingRef);

            //// Act
            var act = model.AwayName;

            //// Assert
            Assert.AreEqual(remotingRef, act);
        }

        [Test]
        public void TurnoverToDisplay_Always_ReturnCorrectFormat()
        {
            //// Arrange
            var correctFormat = "100.00";

            //// Act
            var act = model.TurnoverToDislay;

            //// Assert
            Assert.AreEqual(correctFormat, act);
        }

        [Test]
        public void BankerToDisplay_Always_ReturnCorrectFormat()
        {
            //// Arrange
            var correctFormat = "29.00";

            //// Act
            var act = model.BankerToDisplay;

            //// Assert
            Assert.AreEqual(correctFormat, act);
        }

        [Test]
        public void PlayerToDisplay_Always_ReturnCorrectFormat()
        {
            //// Arrange
            var correctFormat = "39.00";

            //// Act
            var act = model.PlayerToDisplay;

            //// Assert
            Assert.AreEqual(correctFormat, act);
        }
    }
}
