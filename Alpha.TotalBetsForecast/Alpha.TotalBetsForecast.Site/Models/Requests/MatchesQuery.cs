namespace Alpha.TotalBetsForecast.Site.Models.Requests
{
    using System.Collections.Generic;
    using MediatR;

    public class MatchesQuery : IRequest<IEnumerable<Match>>
    {
        public int LeagueId { get; set; }

        public bool IsLive { get; set; }
    }
}