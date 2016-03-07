namespace Alpha.TotalBetsForecast.Site.Criteria
{
    using Fanex.Data.Repository;
    using Sunplus.Agent.Model.BL.Definitions;

    internal class GetEventsCriteria : CriteriaBase
    {
        public int LeagueId { get; set; }

        public bool IsLive { get; set; }

        public bool IsShowAll
        {
            get
            {
                return AppSettings.LocalTest;
            }
        }

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