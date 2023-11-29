namespace Alpha.TotalBetsForecast.Site.Controllers
{
    using System.Web.Mvc;
    using AlphaPlus.User.Models;
    using MediatR;

    public class IntroductionController : TotalBetsForecastBaseController
    {
        private readonly IUserSession userSession;

        public IntroductionController(IMediator mediator, IUserSession userSession)
            : base(mediator)
        {
            this.userSession = userSession;
        }

        [ChildActionOnly]
        public ActionResult Index()
            => View($"Index.{userSession.Language}");

        [ChildActionOnly]
        public ActionResult QuickIntroduction()
            => View("QuickIntroduction");
    }
}