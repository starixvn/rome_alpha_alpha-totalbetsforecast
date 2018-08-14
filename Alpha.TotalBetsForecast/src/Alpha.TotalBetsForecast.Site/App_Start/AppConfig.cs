namespace Alpha.TotalBetsForecast.Site.App_Start
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;

    public static class AppConfig
    {
        [ThreadStatic]
        private static NameValueCollection appSettings;
        private const string DisableHighlightChangeConfigName = "NewTBF:DisableHighlightChange";
        private const string AllowSitesToViewForecastConfigName = "NewTBF:AllowSitesToViewForecast";

        internal static NameValueCollection AppSettings
        {
            get { return appSettings ?? ConfigurationManager.AppSettings; }

            set { appSettings = value; }
        }        

        public static bool DisableHighlightChange => GetValue(DisableHighlightChangeConfigName) == "1";

        public static string AllowSitesToViewForecast => GetValue(AllowSitesToViewForecastConfigName).ToLowerInvariant();

        public static string StoredProcedurePath => GetValue("StoredProcedurePath");

        public static bool EnabledStaticData => GetValue("EnabledStaticData") == "1";

        public static string RemoteRefURL => GetValue("RemoteRefURL");

        public static string GetValue(string key) => AppSettings[key];
    }
}