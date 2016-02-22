namespace Alpha.TotalBetsForecast.Site.Controllers
{
    using System.Web.Mvc;
    using MediatR;
    using Models.Requests;

    public class ForecastController : Controller
    {
        private IMediator _mediator;

        public ForecastController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Forecast
        public ActionResult Index(IndexQuery query)
        {
            var viewModel = _mediator.Send(query);

            return View(viewModel);
        }

        [HttpGet]
        public JsonResult GetMatches(MatchesQuery query)
        {
            var viewModel = _mediator.Send(query);

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult MatchForecast(MatchForecastQuery query)
        {
            var viewModel = _mediator.Send(query);

            return View(viewModel);
        }
    }
}