namespace Alpha.TotalBetsForecast.Site.Controllers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using MediatR;
    using Models.Requests;

    public class ForecastController : TotalBetsForecastBaseController
    {
        private readonly IMediator mediator;

        public ForecastController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Reviewed!")]
        public ActionResult Index(IndexQuery query)
        {
            var viewModel = mediator.Send(query);

            return View(viewModel);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Reviewed!")]
        [HttpGet]
        public JsonResult GetMatches(MatchesQuery query)
        {
            var viewModel = mediator.Send(query);

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Reviewed!")]
        [HttpGet]
        public JsonResult GetMatchForecasts(MatchForecastsQuery query)
        {
            var viewModel = mediator.Send(query);

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Reviewed!")]
        [HttpGet]
        public JsonResult GetLeagues(LeaguesQuery query)
        {
            var viewModel = mediator.Send(query);

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }
    }
}