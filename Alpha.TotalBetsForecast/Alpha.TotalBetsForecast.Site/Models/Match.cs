namespace Alpha.TotalBetsForecast.Site.Models
{
    using System;
    using Sunplus.Agent.Model.RemotingRef;

    public class Match
    {
        public int Id { get; set; }

        public int HomeId { get; set; }

        public bool IsLive { get; set; }

        public string HomeName
        {
            get
            {
                return CacheRefFactory.CacheRef.GetTeamName(HomeId);
            }
        }

        public int AwayId { get; set; }

        public string AwayName
        {
            get
            {
                return CacheRefFactory.CacheRef.GetTeamName(AwayId);
            }
        }

        public byte LiveHomeScore { get; set; }

        public byte LiveAwayScore { get; set; }

        public bool HasLive { get; set; }

        public DateTime KickOffTime { get; set; }

        public string ShowTime { get; set; }

        public string Name
        {
            get
            {
                return $"{HomeName} - {AwayName}";
            }
        }
    }
}