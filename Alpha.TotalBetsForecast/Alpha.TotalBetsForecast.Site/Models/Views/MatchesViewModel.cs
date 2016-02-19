namespace Alpha.TotalBetsForecast.Site.Models.Views
{
    using System.Collections.Generic;

    public class MatchesViewModel
    {
        public int MatchId { get; set; }

        public int LeagueId { get; set; }

        public IEnumerable<Match> Matches { get; set; }
    }
}