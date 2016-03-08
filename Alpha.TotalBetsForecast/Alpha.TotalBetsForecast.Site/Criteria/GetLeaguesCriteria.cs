namespace Alpha.TotalBetsForecast.Site.Criteria
{
    using Fanex.Data.Repository;
    using Sunplus.Agent.Model.BL.Definitions;

    internal class GetLeaguesCriteria : CriteriaBase
    {
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
            return "Age_NewTotalBetsForeCast_LeagueSel";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}