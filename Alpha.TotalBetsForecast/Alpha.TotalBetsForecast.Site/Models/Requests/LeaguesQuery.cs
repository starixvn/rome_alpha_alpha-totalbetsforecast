namespace Alpha.TotalBetsForecast.Site.Models.Requests
{
    using System.Collections.Generic;
    using MediatR;

    public class LeaguesQuery : IRequest<IEnumerable<League>>
    {
        public bool IsLive { get; set; }
    }
}