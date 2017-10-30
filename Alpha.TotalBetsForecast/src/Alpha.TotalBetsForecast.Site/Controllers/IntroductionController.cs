namespace Alpha.TotalBetsForecast.Site.Controllers
{
    using System.Web.Mvc;
    using MediatR;
    using Sunplus.Agent.Model.Entities;

    public class IntroductionController : TotalBetsForecastBaseController
    {
        private readonly IUserSession userSession;
        public IntroductionController(IMediator mediator, IUserSession userSession) : base(mediator)
        {
            this.userSession = userSession;
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