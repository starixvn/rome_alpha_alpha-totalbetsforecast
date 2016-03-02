namespace Alpha.TotalBetsForecast.Site.Criteria
{
    using System;
    using Fanex.Data.Repository;

    internal class GetMatchForecastCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public int MatchId { get; set; }

        public bool IsLive { get; set; }

        public short LiveHomeScore { get; set; }

        public short LiveAwayScore { get; set; }

        public DateTime KickOffTime { get; set; }

        public override string GetSettingKey()
        {
            return "Age_NewTotalBetsForeCast_Main";
        }

        public override bool IsValid()
        {
            return CustId > 0 && MatchId > 0;
        }
    }
}