namespace Alpha.TotalBetsForecast.Site.Controllers
{
    using System.Web.Mvc;
    using MediatR;
    using Sunplus.Agent.Model.Entities;
    using Web;

    public abstract class TotalBetsForecastBaseController : BaseController
    {
        protected readonly IMediator mediator;

        protected TotalBetsForecastBaseController(IMediator mediator, IUserSession userSession)
            : base(userSession)
        {
            this.mediator = mediator;
        }

        protected ActionResult View<TResponse>(IRequest<TResponse> message)
        {
            AssignDarkModeViewData();
            return View(mediator.Send(message));
        }

        protected JsonResult Json<TResponse>(IRequest<TResponse> message)
            => Json(mediator.Send(message), JsonRequestBehavior.AllowGet);
    }
}