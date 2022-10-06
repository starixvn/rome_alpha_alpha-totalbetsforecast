namespace Alpha.TotalBetsForecast.Site.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Alpha.PartialUM.Services;
    using Criteria;
    using Fanex.Caching;
    using Fanex.Common.Helper;
    using Fanex.Data.Repository;
    using Models;

    public class EventService : IEventService
    {
        private const string LeaguesCachingKey = "Alpha.TotalBetsForecast.Site.LeagueCaching_{0}";
        private const int LeaguesCacheTimeOut = 60 * 5; // 5 Minutes

        private readonly IDynamicRepository _dynamicRepository;
        private readonly ICacheService cacheService;
        private readonly IPartialUMService _partialUMService;

        public EventService(
            IDynamicRepository dynamicRepository,
            ICacheService cacheService,
            IPartialUMService partialUMService)
        {
            _dynamicRepository = dynamicRepository;
            this.cacheService = cacheService;
            _partialUMService = partialUMService;
        }

        public IEnumerable<League> GetLeagues(bool isLive, bool isLocalTest)
        {
            var leagues = cacheService.Get<IEnumerable<League>>(string.Format(LeaguesCachingKey, isLive));

            if (leagues == null || !leagues.Any())
            {
                var criteria = new GetLeaguesCriteria(isLive, isLocalTest);

                _partialUMService.ThrowIfUM(criteria.GetSettingKey());

                leagues = _dynamicRepository.Fetch<League>(criteria);

                cacheService.Set(
                    string.Format(LeaguesCachingKey, isLive),
                    leagues,
                    new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(LeaguesCacheTimeOut)));
            }

            return leagues;
        }

        public IEnumerable<Match> GetMatches(int leagueId, bool isLive)
        {
            Check.Argument.IsNotNegativeOrZero(leagueId, nameof(leagueId));

            var criteria = new GetEventsCriteria(leagueId, isLive);

            _partialUMService.ThrowIfUM(criteria.GetSettingKey());

            return _dynamicRepository.Fetch<Match>(criteria);
        }
    }
}