namespace Alpha.TotalBetsForecast.Site.Controllers
{
    using System.Globalization;
    using System.Threading;
    using System.Web.Mvc;
    using System.Web.Mvc.Filters;
    using System.Web.Routing;
    using System.Web.Security;
    using App_Start;
    using Sunplus.Agent.Model.Entities;

    public class BaseController : Controller
    {
        private const string DefaultLanguage = "en-US";

        private static readonly CultureInfo DefaultCulture = CultureInfo.CreateSpecificCulture(DefaultLanguage);

        protected override void OnAuthentication(AuthenticationContext filterContext)
        {
            base.OnAuthentication(filterContext);

            if (UserSession.Current.CustId == 0
                || !HttpContext.User.Identity.IsAuthenticated)
            {
                Response.Redirect(FormsAuthentication.LoginUrl, true);

                return;
            }

            if(!string.IsNullOrWhiteSpace(AppConfig.AllowSitesToViewForecast)
                && !AppConfig.AllowSitesToViewForecast.ToLower().Contains(UserSession.Current.Site.ToLower()))
            {
                Response.Redirect(FormsAuthentication.DefaultUrl, true);

                return;
            }
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = int.MaxValue
            };
        }

        protected override void Initialize(RequestContext requestContext)
        {
            string cultureName = UserSession.Current.Language;
            if (string.IsNullOrEmpty(cultureName))
            {
                cultureName = DefaultLanguage; // default
                UserSession.Current.Language = cultureName;
            }

            // Modify current thread's cultures
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;

            Thread.CurrentThread.CurrentCulture.DateTimeFormat = DefaultCulture.DateTimeFormat;
            Thread.CurrentThread.CurrentCulture.NumberFormat = DefaultCulture.NumberFormat;

            base.Initialize(requestContext);
        }
    }
}