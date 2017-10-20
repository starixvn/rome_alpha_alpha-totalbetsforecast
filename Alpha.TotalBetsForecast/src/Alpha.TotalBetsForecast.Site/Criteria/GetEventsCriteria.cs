namespace Alpha.TotalBetsForecast.Site.Criteria
{
    using Fanex.Data.Repository;
    using Sunplus.Agent.Model.BL.Definitions;

    public class GetEventsCriteria : CriteriaBase
    {
        public GetEventsCriteria(int leagueId, bool isLive, bool isShowAll)
        {
            LeagueId = leagueId;
            IsLive = isLive;
            IsShowAll = isShowAll;
        }

        public int LeagueId { get;}

        public bool IsLive { get; }

        public bool IsShowAll { get; }

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