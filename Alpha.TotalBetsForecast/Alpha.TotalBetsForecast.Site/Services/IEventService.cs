namespace Alpha.TotalBetsForecast.Site.Services
{
    using System.Collections.Generic;
    using Models;

    public interface IEventService
    {
        IEnumerable<League> GetLeagues(bool isLive);

        IEnumerable<Match> GetMatches(int leagueId, bool isLive);

        Match GetMatchInfo(bool isLive, int matchId);
    }
}