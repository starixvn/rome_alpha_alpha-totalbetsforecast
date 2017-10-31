namespace Alpha.TotalBetsForecast.Site.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Alpha.TotalBetsForecast.Models;
    using App_GlobalResources;
    using RemotingRef;

    public class ArenaViewModel
    {
        public ArenaViewModel(IEnumerable<BullfightingModel> Matchs)
        {
            this.Matchs = Matchs;
        }

        public string Name => CacheRefFactory.CacheRef.GetLeagueName3rd(Matchs.FirstOrDefault().LeagueId);

        public string Status => Matchs.FirstOrDefault().HasLive ? Resources.Live : Resources.NonLive;

        public IEnumerable<BullfightingModel> Matchs { get; set; }
    }
}