namespace Alpha.TotalBetsForecast.Tests
{
    using Alpha.TotalBetsForecast.Criteria;
    using Alpha.TotalBetsForecast.Enumerations;
    using Alpha.TotalBetsForecast.Models;
    using NUnit.Framework;

    [TestFixture]
    public class GetBullfightingMatchCriteriaTests
    {
        [TestCase(4, "Age_TotalBets_Bullfighting_Master")]
        [TestCase(3, "Age_TotalBets_Bullfighting_Agent")]
        [TestCase(2, "Age_TotalBets_Bullfighting_Member")]
        public void GetSettingKey_RoleId_ReturnExpectedKey(byte custLevelId, string expectedKey)
        {
            //// Arrange
            var custLevel = Enumeration.FromValue<UserLevel>(custLevelId);
            var criteria = new GetBullfightingMatchCriteria(123, custLevel);

            //// Act
            var act = criteria.GetSettingKey();

            //// Assert
            Assert.AreEqual(expectedKey, act);
        }

        [Test]
        public void IsValid_CustIdMoreThanZero_ReturnTrue()
        {
            //// Arrange
            var criteria = new GetBullfightingMatchCriteria(123, UserLevel.Agent);

            //// Act
            var act = criteria.IsValid();

            //// Assert
            Assert.IsTrue(act);
        }

        [Test]
        public void IsValid_CustIdLessThanZero_ReturnFalse()
        {
            //// Arrange
            var criteria = new GetBullfightingMatchCriteria(0, UserLevel.Agent);

            //// Act
            var act = criteria.IsValid();

            //// Assert
            Assert.IsFalse(act);
        }
    }
}
