namespace Alpha.TotalBetsForecast.Site.Controllers
{
    using System.Globalization;
    using System.Web.Mvc;
    using App_Start;
    using Sunplus.Agent.Model.Entities;
    using Web;

    public class TotalBetsForecastBaseController : BaseController
    {
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