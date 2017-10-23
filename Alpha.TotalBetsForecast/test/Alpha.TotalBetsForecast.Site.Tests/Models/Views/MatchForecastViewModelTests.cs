namespace Alpha.TotalBetsForecast.Site.Tests.Models.Views
{
    using System;
    using Alpha.TotalBetsForecast.Site.Models.Views;
    using NUnit.Framework;

    public class MatchForecastViewModelTests
    {
        [Test]
        public void KickOffTimeString_Always_ReturnCorrectFormat()
        {
            //// Arrange
            var hour = 10;
            var minute = 50;
            var viewModel = new MatchForecastViewModel { KickOffTime = new DateTime(2017, 10, 23, hour, minute, 12) };
            var expectedTime = "10:50 AM";

            //// Act
            var act = viewModel.KickOffTimeString;

            //// Assert
            Assert.AreEqual(expectedTime, act);
        }
    }
}
