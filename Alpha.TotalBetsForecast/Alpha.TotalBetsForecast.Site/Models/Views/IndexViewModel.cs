namespace Alpha.TotalBetsForecast.Site.Models.Views
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class IndexViewModel
    {
        public int LeagueId { get; set; }

        public IEnumerable<SelectListItem> Leagues { get; set; }
    }
}