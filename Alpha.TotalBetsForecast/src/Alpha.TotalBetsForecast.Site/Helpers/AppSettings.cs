using System.Configuration;

namespace Alpha.TotalBetsForecast.Site.Helpers
{
    public static class AppSettings
    {
        public static bool LocalTest = ConfigurationManager.AppSettings["LocalTest"] == "1";
    }
}