namespace Alpha.TotalBetsForecast.Helpers
{
    using System.Threading;
    using Fanex.RemotingRef;

    public static class CacheRefServiceHelper
    {
        private static IRemotingRefService remotingRefService;

        public static string GetLeagueName(int leagueId)
            => Instance.GetLeagueName(leagueId.ToString(), LanguageCode, string.Empty);

        public static string GetLeagueName3rd(int league3rdId)
            => Instance.GetLeagueName3rd(league3rdId.ToString(), LanguageCode, string.Empty);

        public static string GetTeamName(int teamId)
            => Instance.GetTeamName(teamId.ToString(), LanguageCode);

        public static string GetTeamName3rd(int team3rdId)
            => Instance.GetTeamName3rd(team3rdId.ToString(), LanguageCode);

        internal static IRemotingRefService Instance
        {
            get
            {
                remotingRefService = remotingRefService ?? new RemotingRefService();

                return remotingRefService;
            }
            set
            {
                remotingRefService = value;
            }
        }

        private static string LanguageCode
            => Thread.CurrentThread.CurrentCulture.Name;
    }
}
