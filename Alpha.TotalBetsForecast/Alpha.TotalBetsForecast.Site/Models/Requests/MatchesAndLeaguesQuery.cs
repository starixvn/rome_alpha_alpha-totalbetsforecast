namespace Alpha.TotalBetsForecast.Site.Models.Requests
{
    using MediatR;
    using Views;

    public class MatchesAndLeaguesQuery : IRequest<MatchesAndLeaguesViewModel>
    {
        public int MatchId { get; set; }

        public int LeagueId { get; set; }

        public bool IsLive { get; set; }
    }
}