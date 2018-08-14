namespace Alpha.TotalBetsForecast.Site.Models
{
    using TotalBetsForecast.Helpers;

    public class League
    {
        public int Id { get; set; }

        public string Name
        {
            get
            {
                return CacheRefServiceHelper.GetLeagueName(Id);
            }
        }
    }
}