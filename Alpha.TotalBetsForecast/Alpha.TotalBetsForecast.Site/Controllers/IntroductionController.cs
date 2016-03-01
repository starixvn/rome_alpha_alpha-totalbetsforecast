namespace Alpha.TotalBetsForecast.Site.Controllers
{
    using System.Web.Mvc;
    using Sunplus.Agent.Model.Entities;

    public class IntroductionController : BaseController
    {
        [ChildActionOnly]
        public ActionResult Index()
        {
            return View($"Index.{UserSession.Current.Language}");
        }
    }
}