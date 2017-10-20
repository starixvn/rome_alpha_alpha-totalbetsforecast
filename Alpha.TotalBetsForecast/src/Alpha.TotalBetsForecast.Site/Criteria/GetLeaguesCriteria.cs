namespace Alpha.TotalBetsForecast.Site.Criteria
{
    using Fanex.Data.Repository;

    internal class GetLeaguesCriteria : CriteriaBase
    {
        public GetLeaguesCriteria(bool isLive, bool isShowAll)
        {
            IsLive = isLive;
            IsShowAll = isShowAll;
        }

        public bool IsLive { get; }

        public bool IsShowAll { get; }

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