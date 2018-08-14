namespace Alpha.TotalBetsForecast.Tests.Models
{
    using System;
    using Alpha.TotalBetsForecast.Models;
    using Fanex.RemotingRef;
    using Helpers;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class BullfightingModelTests
    {
        private BullfightingModel model;

        [SetUp]
        public void Setup()
        {
            model = new BullfightingModel(new DateTime(2017, month: 10, day: 24), 1, 2, "abc", true, 1, 2, 3, turnover: 100, banker: 29, player: 39);
            CacheRefServiceHelper.Instance = Substitute.For<IRemotingRefService>();
        }

        [Test]
        public void MatchId_Always_EqualWithInConstructor()
        {
            //// Arrange

            //// Act
            var act = model.MatchId;

            //// Assert
            Assert.AreEqual(1, act);
        }

        [Test]
        public void HasLive_Always_EqualWithInConstructor()
        {
            //// Arrange

            //// Act
            var act = model.HasLive;

            //// Assert
            Assert.AreEqual(true, act);
        }

        [Test]
        public void BetCount_Always_EqualWithInConstructor()
        {
            //// Arrange

            //// Act
            var act = model.BetCount;

            //// Assert
            Assert.AreEqual(3, act);
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
        public void HomeName_Always_EqualInRemotingRef()
        {
            //// Arrange
            var remotingRef = "homeabc";
            CacheRefServiceHelper.GetTeamName3rd(model.HomeId).Returns(remotingRef);

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
            CacheRefServiceHelper.GetTeamName3rd(model.AwayId).Returns(remotingRef);

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
