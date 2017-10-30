namespace Alpha.TotalBetsForecast.Site.Controllers
{
    using System.Web.Mvc;
    using MediatR;
    using Web;

    public abstract class TotalBetsForecastBaseController : BaseController
    {
        protected readonly IMediator mediator;

        protected TotalBetsForecastBaseController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        protected ActionResult View<TResponse>(IRequest<TResponse> message)
            => View(mediator.Send(message));

        protected JsonResult Json<TResponse>(IRequest<TResponse> message)
            => Json(mediator.Send(message), JsonRequestBehavior.AllowGet);        
    }
}