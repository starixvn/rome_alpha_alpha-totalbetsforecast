namespace Alpha.TotalBetsForecast.Site.Models
{
    using Alpha.RemotingRef;

    public class League
    {
        public int Id { get; set; }

        public string Name
        {
            get
            {
                return CacheRefFactory.CacheRef.GetLeagueName(Id);
            }
        }
    }
}