namespace Alpha.TotalBetsForecast.Site.Models
{
    using AlphaPlus.Configurations;
    using TotalBetsForecast.Helpers;

    public class League
    {
        public int Id { get; set; }

        public int LeagueGroupId { get; set; }

        public string Name => AppSettings.IsSiteLabel777 ? 
            CacheRefServiceHelper.GetLeagueName(Id, LeagueGroupId) : 
            CacheRefServiceHelper.GetLeagueName(Id);
    }
}