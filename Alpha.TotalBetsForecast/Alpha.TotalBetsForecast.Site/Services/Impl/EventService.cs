namespace Alpha.TotalBetsForecast.Site.Services.Impl
{
    using System.Collections.Generic;
    using System.Linq;
    using Criteria;
    using Fanex.Caching;
    using Fanex.Common.Helper;
    using Fanex.Data.Repository;
    using Models;

    public class EventService : IEventService
    {
        private const string LeaguesCachingKey = " Alpha.TotalBetsForecast.Site.LeagueCaching_{0}";

        private readonly IDynamicRepository _dynamicRepository;
        private readonly ICache _cacheManager;

        public EventService(IDynamicRepository dynamicRepository, ICache cacheManager)
        {
            _dynamicRepository = dynamicRepository;
            _cacheManager = cacheManager;
        }

        public IEnumerable<League> GetLeagues(bool isLive)
        {
            var leagues = _cacheManager.Get<IEnumerable<League>>(string.Format(LeaguesCachingKey, isLive));

            if (leagues == null
                || !leagues.Any())
            {
                leagues =  _dynamicRepository.Fetch<League>(new GetEventsCriteria
                {
                    IsLive = isLive
                });

                _cacheManager.Set(string.Format(LeaguesCachingKey, isLive), leagues, 60 * 15);
            }

            return leagues;
        }

        public IEnumerable<Match> GetMatches(int leagueId, bool isLive)
        {
            Check.Argument.IsNotNegative(leagueId, nameof(leagueId));

            return _dynamicRepository.Fetch<Match>(new GetEventsCriteria
            {
                IsLive = isLive,
                LeagueId = leagueId
            });
        }

        public Match GetMatchInfo(bool isLive, int matchId)
        {
            Check.Argument.IsNotNegativeOrZero(matchId, nameof(matchId));

            return _dynamicRepository.Get<Match>(new GetMatchInfoCriteria
            {
                IsLive = isLive,
                MatchId = matchId
            });
        }
    }
}