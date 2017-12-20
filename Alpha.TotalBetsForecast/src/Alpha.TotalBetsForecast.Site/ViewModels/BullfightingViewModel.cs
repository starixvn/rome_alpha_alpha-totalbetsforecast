namespace Alpha.TotalBetsForecast.Site.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Alpha.TotalBetsForecast.Models;
    using Web.Helpers;

    public class BullfightingViewModel
    {
        public BullfightingViewModel(IEnumerable<BullfightingModel> matchs)
        {
            Arenas = matchs.GroupBy(i => i.LeagueId).Select(g => new ArenaViewModel(g.ToList()));
        }

        public IEnumerable<ArenaViewModel> Arenas { get; }

        public string BuildBetListPath(int matchId)
            => MicroSites.BetListsUrl("~/BullfightingByMatch/Index?matchId=" + matchId);
    }
}