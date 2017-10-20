namespace Alpha.TotalBetsForecast.Site.Models.Requests
{
    using System.Collections.Generic;
    using MediatR;
    using Sunplus.Agent.Model.BL.Definitions;

    public class MatchesQuery : IRequest<IEnumerable<Match>>
    {
        public MatchesQuery(int leagueId, bool isLive) : this(leagueId, isLive, AppSettings.LocalTest)
        {

        }
        public MatchesQuery(int leagueId, bool isLive, bool isLocalTest)
        {
            LeagueId = leagueId;
            IsLive = isLive;
            IsLocalTest = isLocalTest;
        }

        public int LeagueId { get; }

        public bool IsLive { get; }

        public bool IsLocalTest { get; }
    }
}