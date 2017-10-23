namespace Alpha.TotalBetsForecast.Site.Controllers
{
    using System.Web.Mvc;
    using MediatR;
    using Models.Requests;

    public class ForecastController : TotalBetsForecastBaseController
    {
        public ForecastController(IMediator mediator) : base(mediator)
        {
        }

        public ActionResult Index(IndexQuery query) => View(query);

        [HttpGet]
        public JsonResult GetMatches(MatchesQuery query) => Json(query);

        [HttpGet]
        public JsonResult GetMatchForecasts(MatchForecastsQuery query) => Json(query);

        [HttpGet]
        public JsonResult GetLeagues(LeaguesQuery query) => Json(query);
    }
}