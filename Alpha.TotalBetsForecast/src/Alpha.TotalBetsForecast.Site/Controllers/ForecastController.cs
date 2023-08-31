namespace Alpha.TotalBetsForecast.Site.Controllers
{
    using System.Web.Mvc;
    using App_Start;
    using MediatR;
    using Models.Requests;
    using System.Linq;
    using System;
    using AlphaPlus.User.Models;

    public class ForecastController : TotalBetsForecastBaseController
    {
        private readonly IUserSession userSession;

        public ForecastController(IMediator mediator, IUserSession userSession)
            : base(mediator)
        {
            this.userSession = userSession;
        }

        public ActionResult Index(IndexQuery query) => View(query);

        [HttpGet]
        public JsonResult GetMatches(MatchesQuery query) => Json(query);

        [HttpGet]
        public JsonResult GetMatchForecasts(MatchForecastsQuery query) => Json(query);

        [HttpGet]
        public JsonResult GetLeagues(LeaguesQuery query) => Json(query);

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (NotAllowSiteToViewForecast())
            {
                UnAuthentication(filterContext);
            }
        }

        internal bool NotAllowSiteToViewForecast()
            => !string.IsNullOrWhiteSpace(AppConfig.AllowSitesToViewForecast)
            && userSession != null
            && !string.IsNullOrWhiteSpace(userSession.Site)
            && !AppConfig.NewTBFAllowedSites.Any(site => string.Equals(site, "*") || string.Equals(site, userSession.Site, StringComparison.OrdinalIgnoreCase));
    }
}