namespace Alpha.TotalBetsForecast.Site.Criteria
{
    using Fanex.Data.Repository;

    public class GetEventsCriteria : CriteriaBase
    {
        public GetEventsCriteria(int leagueId, bool isLive)
        {
            LeagueId = leagueId;
            IsLive = isLive;
        }

        public int LeagueId { get;}

        public bool IsLive { get; }        

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