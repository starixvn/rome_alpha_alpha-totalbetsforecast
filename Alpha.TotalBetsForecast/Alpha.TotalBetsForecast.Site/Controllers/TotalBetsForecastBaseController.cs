namespace Alpha.TotalBetsForecast.Site.Controllers
{
    using System.Globalization;
    using System.Web.Mvc;
    using App_Start;
    using Sunplus.Agent.Model.Entities;
    using Web;

    public class TotalBetsForecastBaseController : BaseController
    {
        private const string DefaultLanguage = "en-US";

        private static readonly CultureInfo DefaultCulture = CultureInfo.CreateSpecificCulture(DefaultLanguage);

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (!string.IsNullOrWhiteSpace(AppConfig.AllowSitesToViewForecast)
                && !AppConfig.AllowSitesToViewForecast.ToLower().Contains(UserSession.Current.Site.ToLower()))
            {
                UnAuthentication(filterContext);
            }
        }
    }
}