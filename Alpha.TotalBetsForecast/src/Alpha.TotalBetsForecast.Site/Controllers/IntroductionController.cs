namespace Alpha.TotalBetsForecast.Site.Controllers
{
    using System.Web.Mvc;
    using MediatR;
    using Sunplus.Agent.Model.Entities;

    public class IntroductionController : TotalBetsForecastBaseController
    {
        public IntroductionController(IMediator mediator) : base(mediator)
        {
        }

        [ChildActionOnly]
        public ActionResult Index()
        {
            return View($"Index.{UserSession.Current.Language}");
        }

        [ChildActionOnly]
        public ActionResult QuickIntroduction()
        {
            return View();
        }
    }
}