namespace Alpha.TotalBetsForecast.Site.App_Start
{
    using System.Configuration;

    public class AppConfig
    {
        private const string DisableHighlightChangeConfigName = "NewTBF:DisableHighlightChange";
        private const string AllowSitesToViewForecastConfigName = "NewTBF:AllowSitesToViewForecast";

        public static bool DisableHighlightChange
        {
            get
            {
                var disable = GetValue(DisableHighlightChangeConfigName);

                if (disable == null)
                {
                    return false;
                }

                return disable == "1";
            }
        }

        public static string AllowSitesToViewForecast
        {
            get
            {
                return GetValue(AllowSitesToViewForecastConfigName);
            }
        }

        public static string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}