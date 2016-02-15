namespace Alpha.TotalBetsForecast.Site.Services.Impl
{
    using System.Collections.Generic;
    using System.Linq;
    using Criteria;
    using Fanex.Common.Helper;
    using Fanex.Data.Repository;
    using Models;

    public class EventService : IEventService
    {
        private readonly IDynamicRepository _dynamicRepository;

        public EventService(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository;
        }

        public IEnumerable<League> GetLeagues(bool isLive)
        {
            return _dynamicRepository.Fetch<League>(new GetEventsCriteria
            {
                IsLive = isLive
            });
        }

        public IEnumerable<Match> GetMatches(IEnumerable<League> leagues, bool isLive)
        {
            Check.Argument.IsNotNull(leagues, nameof(leagues));

            if (!leagues.Any())
            {
                return Enumerable.Empty<Match>();
            }

            var leagueIds = string.Join(",", leagues.Select(league => league.Id));

            return _dynamicRepository.Fetch<Match>(new GetEventsCriteria
            {
                IsLive = isLive,
                LeagueIds = leagueIds
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