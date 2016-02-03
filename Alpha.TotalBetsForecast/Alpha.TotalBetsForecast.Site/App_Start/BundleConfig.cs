namespace Alpha.TotalBetsForecast.Site.App_Start
{
    using System.Web.Optimization;

    public static class BundleConfig
    {
        public const string NormalResultJavascriptBundleUrl = "~/bundles/scripts/normalresult";
        public const string OutrightResultJavascriptBundleUrl = "~/bundles/scripts/outrightresult";
        public const string ResultCoreStyleBundleUrl = "~/bundle/styles/resultcore";

        public static void RegisterBundles(BundleCollection bundles)
        {
            RegisterJavascriptBundles(bundles);
            RegisterStyleBundles(bundles);
        }

        private static void RegisterJavascriptBundles(BundleCollection bundles)
        {
        }

        private static void RegisterStyleBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle(ResultCoreStyleBundleUrl)
                .Include("~/Content/Fanex.Result/fanex.result.css", new CssRewriteUrlTransformWrapper())
                .Include("~/Content/result.css", new CssRewriteUrlTransformWrapper()));
        }
    }
}