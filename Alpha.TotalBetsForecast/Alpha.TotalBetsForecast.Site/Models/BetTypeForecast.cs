namespace Alpha.TotalBetsForecast.Site.Models
{
    using System.Collections.Generic;
    using Sunplus.Agent.Model.RemotingRef;

    public class BetTypeForecast
    {
        public int BetTypeId { get; set; }

        public string BetTypeName
        {
            get
            {
                return CacheRefFactory.CacheRef.GetBettype(BetTypeId);
            }
        }

        public string BetType { get; set; }

        public IEnumerable<BetChoiceForecast> BetChoices { get; set; }

        public bool IsOneXTwoBetType
        {
            get
            {
                return BetTypeId == (int)Sunplus.Agent.Model.BL.Definitions.BetType.FT_1X2
                            || BetTypeId == (int)Sunplus.Agent.Model.BL.Definitions.BetType.FirstHalf_1X2;
            }
        }

    }
}