namespace Alpha.TotalBetsForecast.Site.Controllers
{
    using System.Web.Mvc;
    using Alpha.TotalBetsForecast.Site.Requests;
    using MediatR;

    public class BullfightingController : TotalBetsForecastBaseController
    {
        public BullfightingController(IMediator mediator) : base(mediator)
        {
        }

        public ActionResult Index() => View("Index");

        public ActionResult GetMatches(BullfightingQuery query) => View(query);
    }
}