namespace Alpha.TotalBetsForecast.Site.Criteria
{
    using Fanex.Data.Repository;

    public class GetEventsCriteria : CriteriaBase
    {
        public int LeagueId { get; set; }

        public bool IsLive { get; set; }

        public override string GetSettingKey()
        {
            return "Age_NewTotalBetsForeCast_EventSel";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}