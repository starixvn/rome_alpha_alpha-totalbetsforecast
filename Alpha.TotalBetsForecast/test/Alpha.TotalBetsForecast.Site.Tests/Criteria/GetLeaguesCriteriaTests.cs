namespace Alpha.TotalBetsForecast.Site.Tests.Criteria
{
    using Alpha.TotalBetsForecast.Site.Criteria;
    using NUnit.Framework;

    public class GetLeaguesCriteriaTests
    {
        [Test]
        public void IsShowAll_Always_EqualWithContrucstor()
        {
            //// Arrange
            var expectedValue = true;
            var criteria = new GetLeaguesCriteria(true, expectedValue);

            //// Act
            var act = criteria.IsShowAll;

            //// Assert
            Assert.AreEqual(expectedValue, act);
        }

        [Test]
        public void GetSettingKey_Always_ReturnCorrectKey()
        {
            //// Arrange
            var expectedKey = "Age_NewTotalBetsForeCast_LeagueSel";
            var criteria = new GetLeaguesCriteria(true, true);

            //// Act
            var act = criteria.GetSettingKey();

            //// Assert
            Assert.AreEqual(expectedKey, act);
        }

        [Test]
        public void IsValid_Always_ReturnTrue()
        {
            //// Arrange
            var criteria = new GetLeaguesCriteria(true, true);

            //// Act
            var act = criteria.IsValid();

            //// Assert
            Assert.IsTrue(act);
        }
    }
}
