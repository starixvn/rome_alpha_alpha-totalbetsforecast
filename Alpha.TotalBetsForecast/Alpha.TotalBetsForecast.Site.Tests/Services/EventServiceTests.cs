namespace Alpha.TotalBetsForecast.Site.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using Criteria;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Models;
    using NSubstitute;
    using NUnit.Framework;
    using Site.Services.Impl;

    [TestFixture]
    public class EventServiceTests
    {
        private IDynamicRepository _dynamicRepository;
        private ICache _cacheManager;
        private EventService _eventService;

        [SetUp]
        public void SetUp()
        {
            _dynamicRepository = Substitute.For<IDynamicRepository>();
            _cacheManager = Substitute.For<ICache>();

            _eventService = new EventService(_dynamicRepository, _cacheManager);
        }

        #region Get Leagues

        [TestCase(true)]
        [TestCase(false)]
        public void GetLeagues_WhenCalled_QueryInCacheFirst(bool isLive)
        {
            //// Act
            _eventService.GetLeagues(isLive);

            //// Assert
            _cacheManager.Received().Get<IEnumerable<League>>($"Alpha.TotalBetsForecast.Site.LeagueCaching_{isLive}");
        }

        [Test]
        public void GetLeague_CacheHasData_ReturnCachedLeagues()
        {
            //// Arrange
            var expectedLeague = new List<League>
            {
                new League
                {
                    Id = 1
                }
            };
            _cacheManager.Get<IEnumerable<League>>(Arg.Any<string>()).Returns(expectedLeague);

            //// Act
            var actualLeagues = _eventService.GetLeagues(false);

            //// Assert
            Assert.AreEqual(expectedLeague, actualLeagues);
        }

        [Test]
        public void GetLeagues_NoCache_ReturnLeaguesFromRepository()
        {
            //// Arrange
            var expectedLeague = new List<League>
            {
                new League
                {
                    Id = 1
                }
            };
            IEnumerable<League> cachedLeagues = null;
            _cacheManager.Get<IEnumerable<League>>(Arg.Any<string>()).Returns(cachedLeagues);
            _dynamicRepository.Fetch<League>(Arg.Is<GetLeaguesCriteria>(
                criteria => !criteria.IsLive)).Returns(expectedLeague);

            //// Act
            var actualLeagues = _eventService.GetLeagues(false);

            //// Assert
            Assert.AreEqual(expectedLeague, actualLeagues);
        }

        #endregion Get Leagues

        #region Get Matches

        [TestCase(0)]
        [TestCase(-1)]
        public void GetMatches_LeagueIdIsNegativeOrZero_ThrowArgumentOutOfRangeException(int leagueId)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _eventService.GetMatches(leagueId, true));
        }

        [Test]
        public void GetMatches_ReturnMatchesFromRepository()
        {
            var leagueId = 1;
            var isLive = true;
            var expectedMatches = new List<Match>
            {
                new Match
                {
                    Id = 1
                }
            };
            _dynamicRepository.Fetch<Match>(Arg.Is<GetEventsCriteria>(
                criteria => criteria.LeagueId == leagueId
                        && criteria.IsLive == isLive)).Returns(expectedMatches);

            //// Act
            var actualMatches = _eventService.GetMatches(leagueId, isLive);

            //// Assert
            Assert.AreEqual(expectedMatches, actualMatches);
        }

        #endregion Get Matches
    }
}