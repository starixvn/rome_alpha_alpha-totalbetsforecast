namespace Alpha.TotalBetsForecast.Tests.Models
{
    using System;
    using Alpha.TotalBetsForecast.Enumerations;
    using Alpha.TotalBetsForecast.Models;
    using NUnit.Framework;

    [TestFixture]
    public class UserLevelTests
    {
        [Test]
        public void IsAgentLevel_UserLevelIsAgent_ReturnsTrue()
        {
            // Arrange
            var userLevel = Enumeration.FromValue<UserLevel>(UserLevel.Agent.Value);

            // Act
            var result = userLevel.IsAgentLevel();

            // Assert
            Assert.AreEqual(true, result);
        }

        [Test]
        public void IsNotAgentLevel_UserLevelIsAgent_ReturnsFalse()
        {
            // Arrange
            var userLevel = Enumeration.FromValue<UserLevel>(UserLevel.Agent.Value);

            // Act
            var result = userLevel.IsNotAgentLevel();

            // Assert
            Assert.AreEqual(false, result);
        }

        [Test]
        public void IsMasterLevel_UserLevelIsMaster_ReturnsTrue()
        {
            // Arrange
            var userLevel = Enumeration.FromValue<UserLevel>(UserLevel.Master.Value);

            // Act
            var result = userLevel.IsMasterLevel();

            // Assert
            Assert.AreEqual(true, result);
        }

        [Test]
        public void IsNotMasterLevel_UserLevelIsMaster_ReturnsFalse()
        {
            // Arrange
            var userLevel = Enumeration.FromValue<UserLevel>(UserLevel.Master.Value);

            // Act
            var result = userLevel.IsNotMasterLevel();

            // Assert
            Assert.AreEqual(false, result);
        }

        [Test]
        public void IsSuperLevel_UserLevelIsSuper_ReturnsTrue()
        {
            // Arrange
            var userLevel = Enumeration.FromValue<UserLevel>(UserLevel.Super.Value);

            // Act
            var result = userLevel.IsSuperLevel();

            // Assert
            Assert.AreEqual(true, result);
        }

        [Theory]
        [TestCase(4)]
        [TestCase(3)]
        public void IsMasterOrSuperLevel_UserLevelIsSuperOrMaster_ReturnsTrue(byte levelId)
        {
            // Arrange
            var userLevel = Enumeration.FromValue<UserLevel>(levelId);

            // Act
            var result = userLevel.IsMasterOrSuperLevel();

            // Assert
            Assert.AreEqual(true, result);
        }

        [Test]
        public void IsMemberLevel_UserLevelIsMember_ReturnsTrue()
        {
            // Arrange
            var userLevel = Enumeration.FromValue<UserLevel>(UserLevel.Member.Value);

            // Act
            var result = userLevel.IsMemberLevel();

            // Assert
            Assert.AreEqual(true, result);
        }

        [Test]
        public void IsNotSuperLevel_UserLevelIsSuper_ReturnsFalse()
        {
            // Arrange
            var userLevel = Enumeration.FromValue<UserLevel>(UserLevel.Super.Value);

            // Act
            var result = userLevel.IsNotSuperLevel();

            // Assert
            Assert.AreEqual(false, result);
        }

        [Test]
        public void IsDirectUplineOf_CustomerLevelIsNull_ReturnsFalse()
        {
            // Arrange
            var userLevel = Enumeration.FromValue<UserLevel>(UserLevel.Super.Value);

            // Act
            var result = userLevel.IsDirectUplineOf(null);

            // Assert
            Assert.AreEqual(false, result);
        }


        [Test]
        public void IsDirectDownlineOf_CustomerLevelIsNull_ReturnsFalse()
        {
            // Arrange
            var userLevel = Enumeration.FromValue<UserLevel>(UserLevel.Super.Value);

            // Act
            var result = userLevel.IsDirectDownlineOf(null);

            // Assert
            Assert.AreEqual(false, result);
        }

        [TestCase(4)]
        [TestCase(3)]
        [TestCase(2)]
        public void Is_UserLevelIsNull_ReturnsFalse(byte userLevelValue)
        {
            // Arrange
            var userLevel = Enumeration.FromValue<UserLevel>(userLevelValue);

            // Act
            var result = userLevel.Is(null);

            // Assert
            Assert.AreEqual(false, result);
        }


        [TestCase(4, 3)]
        [TestCase(3, 2)]
        [TestCase(2, 1)]
        public void GetDirectDownLineLevel_SMALevel_ReturnsDirectDownline(byte roleId, byte expectedDirectDownlineRoleId)
        {
            // Arrange
            var userLevel = Enumeration.FromValue<UserLevel>(roleId);

            // Act
            var actualDirectDownlineRoleId = userLevel.GetDirectDownLineLevel().Value;

            // Assert
            Assert.AreEqual(expectedDirectDownlineRoleId, actualDirectDownlineRoleId);
        }

        [Test]
        public void GetDirectDownLineLevel_MemberLevel_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var userLevel = Enumeration.FromValue<UserLevel>(1);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => userLevel.GetDirectDownLineLevel());
        }

        [Test]
        public void IsUplineOf_GapBetweenTwoLevelsLessThan1_ReturnsFalse()
        {
            // Arrange
            var userLevel = UserLevel.Master;

            // Act
            var actualIsUplineOf = userLevel.IsUplineOf(4);

            // Assert
            Assert.False(actualIsUplineOf);
        }

        [Test]
        public void IsUplineOf_GapBetweenTwoLevelsFrom1_ReturnsTrue()
        {
            // Arrange
            var userLevel = UserLevel.Master;

            // Act
            var actualIsUplineOf = userLevel.IsUplineOf(2);

            // Assert
            Assert.True(actualIsUplineOf);
        }

        [Test]
        public void IsUplineOf_DownlineLevelIsNull_ReturnsFalse()
        {
            // Arrange
            var userLevel = UserLevel.Master;
            UserLevel downlineLevel = null;

            // Act
            var actualIsUplineOf = userLevel.IsUplineOf(downlineLevel);

            // Assert
            Assert.False(actualIsUplineOf);
        }

        [Test]
        public void IsUplineOf_ValidDownline_ReturnsTrue()
        {
            // Arrange
            var userLevel = UserLevel.Master;
            var downlineLevel = UserLevel.Agent;

            // Act
            var actualIsUplineOf = userLevel.IsUplineOf(downlineLevel);

            // Assert
            Assert.True(actualIsUplineOf);
        }
    }
}
