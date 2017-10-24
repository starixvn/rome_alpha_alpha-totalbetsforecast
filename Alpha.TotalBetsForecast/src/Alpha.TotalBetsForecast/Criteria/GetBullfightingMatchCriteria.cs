namespace Alpha.TotalBetsForecast.Criteria
{
    using Fanex.Data.Repository;
    using Models;

    public class GetBullfightingMatchCriteria : CriteriaBase
    {        
        public GetBullfightingMatchCriteria(int custId, UserLevel custLevel)
        {
            this.custId = custId;
            this.CustLevel = custLevel;
        }

        public int custId { get; }

        private UserLevel CustLevel { get; }

        public override bool IsValid() => custId > 0;

        public override string GetSettingKey()
            => CustLevel.IsAgentLevel() ?
            "Age_TotalBets_Bullfighting_Member" :
            CustLevel.IsMasterLevel() ?
            "Age_TotalBets_Bullfighting_Agent" :
            "Age_TotalBets_Bullfighting_Master";
    }
}