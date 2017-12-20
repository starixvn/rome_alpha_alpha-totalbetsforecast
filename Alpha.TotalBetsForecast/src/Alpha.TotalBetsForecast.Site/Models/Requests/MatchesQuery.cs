namespace Alpha.TotalBetsForecast.Site.Models.Requests
{
    using System.Collections.Generic;
    using MediatR;
    using Sunplus.Agent.Model.BL.Definitions;

    public class MatchesQuery : IRequest<IEnumerable<Match>>
    {
        public int LeagueId { get; set; }

        public bool IsLive { get; set; }        
    }
}