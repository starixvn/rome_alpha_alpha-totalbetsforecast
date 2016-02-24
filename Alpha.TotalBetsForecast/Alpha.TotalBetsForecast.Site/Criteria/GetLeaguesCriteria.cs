namespace Alpha.TotalBetsForecast.Site.Criteria
{
    using Fanex.Data.Repository;

    public class GetLeaguesCriteria : CriteriaBase
    {
        public bool IsLive { get; set; }

        public override string GetSettingKey()
        {
            return "Age_NewTotalBetsForeCast_LeagueSel";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}