namespace Alpha.TotalBetsForecast.Site.Helpers
{
    using System.Configuration;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Web;
    using System.Web.Configuration;

    public static class StaticResources
    {
        private static readonly string StaticResourcesVersion;

        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate", Justification = "Reviewed!")]
        private static readonly string MinText = ".min";

        private static readonly string AssetAppPath;

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Reviewed!")]
        static StaticResources()
        {
            if (IsDebugMode())
            {
                MinText = string.Empty;
            }

            StaticResourcesVersion = GetBuildVersion();
            AssetAppPath = ConfigurationManager.AppSettings["AssetAppPath"];
        }

        private static string GetBuildVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            return versionInfo.FileVersion;
        }

        private static bool IsDebugMode()
        {
            var compilationSection = (CompilationSection)ConfigurationManager.GetSection(@"system.web/compilation");

            return compilationSection.Debug;
        }

        #region Assets App

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Reviewed!")]
        public static class AssetStyles
        {
            public static IHtmlString Render(params string[] paths)
            {
                return System.Web.Optimization.Styles.Render(paths.Select(path => $"{AssetAppPath}{path}{MinText}.css?v={StaticResourcesVersion}").ToArray());
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Reviewed!")]
        public static class AssetScripts
        {
            public static IHtmlString Render(params string[] paths)
            {
                return System.Web.Optimization.Scripts.Render(paths.Select(path => $"{AssetAppPath}{path}{MinText}.js?v={StaticResourcesVersion}").ToArray());
            }
        }

        #endregion Assets App
    }
}