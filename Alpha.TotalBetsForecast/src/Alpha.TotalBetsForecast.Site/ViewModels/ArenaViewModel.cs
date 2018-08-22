namespace Alpha.TotalBetsForecast.Site.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Alpha.TotalBetsForecast.Models;
    using App_GlobalResources;
    using TotalBetsForecast.Helpers;

    public class ArenaViewModel
    {
        public ArenaViewModel(IEnumerable<BullfightingModel> Matchs)
        {
            this.Matchs = Matchs;
            MatchSummary = this.Matchs.FirstOrDefault();
        }

        public string Name
            => MatchSummary == null ? string.Empty : $"{MatchSummary.LeagueCode} - {CacheRefServiceHelper.GetLeagueName3rd(MatchSummary.LeagueId)}";

        public string Status
            => MatchSummary == null ? string.Empty : MatchSummary.HasLive ? Resources.Live : Resources.NonLive;

        public IEnumerable<BullfightingModel> Matchs { get; set; }

        private readonly BullfightingModel MatchSummary;
    }
}