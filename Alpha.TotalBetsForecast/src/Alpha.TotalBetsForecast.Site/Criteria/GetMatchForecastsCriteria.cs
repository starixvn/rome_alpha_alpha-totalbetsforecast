namespace Alpha.TotalBetsForecast.Site.Criteria
{
    using Fanex.Data.Repository;

    internal class GetMatchForecastsCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        [SpParam(SqlParamName = "MatchIDList")]
        public string MatchIds { get; set; }

        public bool IsLive { get; set; }

        public override string GetSettingKey()
        {
            return "Age_NewTotalBetsForeCast_Main";
        }

        public override bool IsValid()
        {
            return CustId > 0 && !string.IsNullOrWhiteSpace(MatchIds);
        }
    }
}