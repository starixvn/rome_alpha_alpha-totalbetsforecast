namespace Alpha.TotalBetsForecast.Site
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using App_Start;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Extensions.Web;
    using ONEbook.UM.Filters;
    using Sunplus.Agent.Model.Entities;
    using Web;

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            MvcHandler.DisableMvcResponseHeader = true;
            DependencyConfig.Init();

            Fanex.Data.Configuration.DbSettingProviderManager.Current.Start();

            ShareSession.Init();

            GlobalFilters.Filters.Add(new CheckUMFilter());

            var logConfig = new LoggingConfiguration()
                .GetHttpHeaderInfo()
                .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);

            ExceptionHandler.UseConfig(logConfig);
        }

        /// <summary>
        /// Application error event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        protected void Application_Error(object sender, EventArgs e)
        {
            if (Request.IsLocal)
            {
                return;
            }

            var currentCustomer = UserSession.Current;

            ExceptionHandler.Handle(
                Server.GetLastError(),
                new Dictionary<string, object>
                {
                    ["CustId"] = currentCustomer?.CustId,
                    ["CustName"] = currentCustomer?.UserName,
                    ["SubAccountName"] = currentCustomer?.SubUserName,
                    ["AdminName"] = currentCustomer?.AdminName
                });
        }
    }
}
