namespace Alpha.TotalBetsForecast.Site.Models.Views
{
    using System.Collections.Generic;

    public class MatchesAndLeaguesViewModel
    {
        public int MatchId { get; set; }

        public int LeagueId { get; set; }

        public IEnumerable<Match> Matches { get; set; }

        public IEnumerable<League> Leagues { get; set; }
    }
}