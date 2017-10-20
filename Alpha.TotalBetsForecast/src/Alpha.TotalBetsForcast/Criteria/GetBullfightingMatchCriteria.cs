namespace Alpha.TotalBetsForcast.Criteria
{
    using Fanex.Data.Repository;

    public class GetBullfightingMatchCriteria : CriteriaBase
    {        
        public GetBullfightingMatchCriteria(int custId)
        {
            this.custId = custId;
        }

        public int custId { get; }

        public override bool IsValid() => custId > 0;

        public override string GetSettingKey() => "";
    }
}