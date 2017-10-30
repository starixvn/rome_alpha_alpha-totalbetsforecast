namespace Alpha.TotalBetsForecast.Site.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Alpha.TotalBetsForecast.Models;

    public class BullfightingViewModel
    {
        public BullfightingViewModel(IEnumerable<BullfightingModel> matchs)
        {
            Arenas = matchs.GroupBy(i => i.LeagueId).Select(g => new ArenaViewModel(g.ToList()));
        }

        public IEnumerable<ArenaViewModel> Arenas { get; }
    }    
}