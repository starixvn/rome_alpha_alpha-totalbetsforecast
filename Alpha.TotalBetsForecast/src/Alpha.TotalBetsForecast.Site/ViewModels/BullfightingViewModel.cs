namespace Alpha.TotalBetsForecast.Site.ViewModels
{
    using System.Collections.Generic;
    using Alpha.TotalBetsForecast.Models;

    public class BullfightingViewModel
    {
        public BullfightingViewModel(IEnumerable<BullfightingModel> matchs)
        {
            Matchs = matchs;
        }

        public IEnumerable<BullfightingModel> Matchs { get; }
    }
}