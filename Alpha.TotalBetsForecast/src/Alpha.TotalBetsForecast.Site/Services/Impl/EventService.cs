namespace Alpha.TotalBetsForecast.Site.Services.Impl
{
    using System.Collections.Generic;
    using System.Linq;
    using Alpha.Web.Helpers;
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
        private readonly ICache _cacheManager;
        private readonly PartialUMService _partialUMService;

        public EventService(
            IDynamicRepository dynamicRepository,
            ICache cacheManager,
            PartialUMService partialUMService)
        {
            _dynamicRepository = dynamicRepository;
            _cacheManager = cacheManager;
            _partialUMService = partialUMService;
        }

        public IEnumerable<League> GetLeagues(bool isLive, bool isLocalTest)
        {
            var leagues = _cacheManager.Get<IEnumerable<League>>(string.Format(LeaguesCachingKey, isLive));

            if (leagues == null || !leagues.Any())
            {
                var criteria = new GetLeaguesCriteria(isLive, isLocalTest);

                _partialUMService.CheckPartialUM(criteria.GetSettingKey());

                leagues = _dynamicRepository.Fetch<League>(criteria);

                _cacheManager.Set(string.Format(LeaguesCachingKey, isLive), leagues, LeaguesCacheTimeOut);
            }

            return leagues;
        }

        public IEnumerable<Match> GetMatches(int leagueId, bool isLive)
        {
            Check.Argument.IsNotNegativeOrZero(leagueId, nameof(leagueId));

            var criteria = new GetEventsCriteria(leagueId, isLive);

            _partialUMService.CheckPartialUM(criteria.GetSettingKey());

            return _dynamicRepository.Fetch<Match>(criteria);
        }
    }
}