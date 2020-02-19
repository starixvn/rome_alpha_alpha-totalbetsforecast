namespace Alpha.TotalBetsForecast.Site.Tests.Criteria
{
    using Alpha.TotalBetsForecast.Site.Criteria;
    using NUnit.Framework;

    public class GetMatchForecastsCriteriaTests
    {
        [Test]
        public void GetSettingKey_Always_ReturnExpectedKey()
        {
            //// Arrange
            var expectedKey = "Ost_NewTotalBetsForeCast_Main";
            var criteria = new GetMatchForecastsCriteria();

            //// Act
            var act = criteria.GetSettingKey();

            //// Assert
            Assert.AreEqual(expectedKey, act);
        }

        [Test]
        public void IsValid_CorrectData_ReturnTrue()
        {
            //// Arrange
            var criteria = new GetMatchForecastsCriteria { CustId = 123, MatchIds = "1212,121" };

            //// Act
            var act = criteria.IsValid();

            //// Assert
            Assert.IsTrue(act);
        }

        [Test]
        public void IsValid_IncorrectData_ReturnFalse()
        {
            //// Arrange
            var criteria = new GetMatchForecastsCriteria { CustId = -123, MatchIds = string.Empty };

            //// Act
            var act = criteria.IsValid();

            //// Assert
            Assert.IsFalse(act);
        }
    }
}
