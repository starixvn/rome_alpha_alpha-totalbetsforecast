namespace Alpha.TotalBetsForecast.Site.ViewModel
{
    using System.Collections.Generic;
    using Alpha.TotalBetsForcast.Model;

    public class BullfightingViewModel
    {
        public BullfightingViewModel(IEnumerable<BullfightingModel> matchs)
        {
            Matchs = matchs;
        }

        private IEnumerable<BullfightingModel> Matchs { get; }
    }
}