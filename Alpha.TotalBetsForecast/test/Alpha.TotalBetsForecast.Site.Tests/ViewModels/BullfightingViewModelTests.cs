namespace Alpha.TotalBetsForecast.Site.Tests.ViewModels
{
    using System.Linq;
    using Alpha.TotalBetsForecast.Models;
    using Alpha.TotalBetsForecast.Site.ViewModels;
    using NUnit.Framework;
    using Ploeh.AutoFixture;

    public class BullfightingViewModelTests
    {
        [Test]
        public void Matchs_Always_EqualWithInConstructor()
        {
            //// Arrange
            var fixture = new Fixture();
            var matchs = fixture.CreateMany<BullfightingModel>();
            var viewModel = new BullfightingViewModel(matchs);

            //// Act
            var act = viewModel.Arenas;

            //// Assert
            Assert.AreEqual(matchs.Count(), act.Count());
        }
    }
}
