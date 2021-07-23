namespace Alpha.TotalBetsForecast.Site.Controllers
{
    using System.Web.Mvc;
    using MediatR;
    using Sunplus.Agent.Model.Entities;

    public class IntroductionController : TotalBetsForecastBaseController
    {
        public IntroductionController(IMediator mediator, IUserSession userSession)
            : base(mediator, userSession)
        {
        }

        [ChildActionOnly]
        public ActionResult Index()
        {
            return View($"Index.{userSession.Language}");
        }

        [ChildActionOnly]
        public ActionResult QuickIntroduction()
        {
            return View("QuickIntroduction");
        }
    }
}