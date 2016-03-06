namespace Alpha.TotalBetsForecast.Site.Models.Views
{
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class IndexViewModel
    {
        public bool IsLive { get; set; }

        public IEnumerable<SelectListItem> LiveValues { get; set; }
    }
}