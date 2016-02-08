namespace Alpha.TotalBetsForecast.Site.Criteria
{
    using Fanex.Data.Repository;

    public class GetMatchInfoCriteria : CriteriaBase
    {
        public int MatchId { get; set; }

        public bool IsLive { get; set; }

        public override string GetSettingKey()
        {
            return "Age_NewTotalBetsForeCast_GetMatchInfo";
        }

        public override bool IsValid()
        {
            return MatchId > 0;
        }
    }
}