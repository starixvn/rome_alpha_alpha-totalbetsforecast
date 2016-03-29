namespace Alpha.TotalBetsForecast.Site.App_Start
{
    using System.Web.Optimization;

    public static class BundleConfig
    {
        public const string IndexForecastJavascriptBundleUrl = "~/bundles/scripts/forecast/index";
        public const string OutrightResultJavascriptBundleUrl = "~/bundles/scripts/outrightresult";
        public const string IndexForecastStyleBundleUrl = "~/bundle/styles/forecast/index";

        public static void RegisterBundles(BundleCollection bundles)
        {
            RegisterJavascriptBundles(bundles);
            RegisterStyleBundles(bundles);
        }

        private static void RegisterJavascriptBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle(IndexForecastJavascriptBundleUrl)
                .Include("~/Scripts/Forecast/forecast-renderer.js")
                .Include("~/Scripts/Forecast/index.js")
                .Include("~/Scripts/Introduction/quick-introduction.js"));
        }

        private static void RegisterStyleBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle(IndexForecastStyleBundleUrl)
                .Include("~/Content/totalbets-forecast.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/quick-introduction.css", new CssRewriteUrlTransformWrapper()));
        }
    }
}