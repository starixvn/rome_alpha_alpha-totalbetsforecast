namespace Alpha.TotalBetsForecast.Site.Criteria
{
    using Fanex.Data.Repository;

    public class GetMatchesCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "LeagueList")]
        public string LeagueIds { get; set; }

        public bool IsLive { get; set; }

        public override string GetSettingKey()
        {
            return "Age_NewTotalBetsForeCast_EventSel";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(LeagueIds);
        }
    }
}