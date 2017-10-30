namespace Alpha.TotalBetsForecast.Site.Services.Impl
{
    using System.Collections.Generic;
    using System.Linq;
    using Criteria;
    using Fanex.Caching;
    using Fanex.Common.Helper;
    using Fanex.Data.Repository;
    using Models;
    using Sunplus.Agent.Model.BL.Definitions;

    public class EventService : IEventService
    {
        private const string LeaguesCachingKey = "Alpha.TotalBetsForecast.Site.LeagueCaching_{0}";
        private const int LeaguesCacheTimeOut = 60 * 5; // 5 Minutes

        private readonly IDynamicRepository _dynamicRepository;
        private readonly ICache _cacheManager;

        public EventService(IDynamicRepository dynamicRepository, ICache cacheManager)
        {
            _dynamicRepository = dynamicRepository;
            _cacheManager = cacheManager;
        }

        public IEnumerable<League> GetLeagues(bool isLive, bool isLocalTest)
        {
            var leagues = _cacheManager.Get<IEnumerable<League>>(string.Format(LeaguesCachingKey, isLive));

            if (leagues == null
                || !leagues.Any())
            {
                leagues = _dynamicRepository.Fetch<League>(new GetLeaguesCriteria(isLive, isLocalTest));

                _cacheManager.Set(string.Format(LeaguesCachingKey, isLive), leagues, LeaguesCacheTimeOut);
            }

            return leagues;
        }

        public IEnumerable<Match> GetMatches(int leagueId, bool isLive)
        {
            Check.Argument.IsNotNegativeOrZero(leagueId, nameof(leagueId));

            return _dynamicRepository.Fetch<Match>(new GetEventsCriteria(leagueId, isLive));
        }
    }
}