namespace Alpha.TotalBetsForecast.Site.Tests.Criteria
{
    using Alpha.TotalBetsForecast.Site.Criteria;
    using NUnit.Framework;

    public class GetEventsCriteriaTests
    {
        [Test]
        public void LeagueId_Always_EqualWithInConstructor()
        {
            //// Arrange
            var leagueId = 123;
            var criteria = new GetEventsCriteria(leagueId, true);

            //// Act
            var act = criteria.LeagueId;

            //// Assert
            Assert.AreEqual(leagueId, act);
        }

        [Test]
        public void IsLive_Always_EqualWithInConstructor()
        {
            //// Arrange
            var isLive = true;
            var criteria = new GetEventsCriteria(123, isLive);

            //// Act
            var act = criteria.IsLive;

            //// Assert
            Assert.AreEqual(isLive, act);
        }
       

        [Test]
        public void GetSettingKey_Always_ExpectedKey()
        {
            //// Arrange
            var expectedKey = "Age_NewTotalBetsForeCast_EventSel";
            var criteria = new GetEventsCriteria(123, true);

            //// Act
            var act = criteria.GetSettingKey();

            //// Assert
            Assert.AreEqual(expectedKey, act);
        }

        [Test]
        public void IsValid_Always_ExpectedTrue()
        {
            //// Arrange
            var criteria = new GetEventsCriteria(123, true);

            //// Act
            var act = criteria.IsValid();

            //// Assert
            Assert.IsTrue(act);
        }
    }
}
