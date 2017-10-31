namespace Alpha.TotalBetsForecast.Site.Tests.ViewModels
{
    using System;
    using System.Collections.Generic;
    using Alpha.TotalBetsForecast.Models;
    using Alpha.TotalBetsForecast.Site.ViewModels;
    using NSubstitute;
    using NUnit.Framework;
    using Ploeh.AutoFixture;
    using RemotingRef;

    public class ArenaViewModelTests
    {
        private Fixture fixture;

        private void Setup()
        {
            fixture = new Fixture();
            CacheRefFactory.CacheRef = NSubstitute.Substitute.For<CacheRefService>();
        }

        [Test]
        public void Name_Always_GetFromRemotingRef()
        {
            //// Arrange            
            var league = new { Id = fixture.Create<int>(), Name = fixture.Create<string>() };
            var matchs = new List<BullfightingModel> { new BullfightingModel(fixture.Create<DateTime>(), fixture.Create<int>(), league.Id, fixture.Create<bool>(), fixture.Create<int>(), fixture.Create<int>(), fixture.Create<int>(), fixture.Create<decimal>(), fixture.Create<decimal>(), fixture.Create<decimal>()) };
            var viewModel = new ArenaViewModel(matchs);            
            CacheRefFactory.CacheRef.GetLeagueName3rd(league.Id).Returns(league.Name);

            //// Act
            var act = viewModel.Name;

            //// Assert
            Assert.AreEqual(league.Name, act);
        }
    }
}
