namespace Alpha.TotalBetsForecast.Site.Controllers
{
    using System.Web.Mvc;
    using App_Start;
    using MediatR;
    using Sunplus.Agent.Model.Entities;
    using Web;

    public class TotalBetsForecastBaseController : BaseController
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

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            var isContain = AppConfig.AllowSitesToViewForecast.Contains(UserSession.Current.Site.ToLowerInvariant());

            if (!string.IsNullOrWhiteSpace(AppConfig.AllowSitesToViewForecast) && !isContain)
            {
                UnAuthentication(filterContext);
            }
        }
    }
}