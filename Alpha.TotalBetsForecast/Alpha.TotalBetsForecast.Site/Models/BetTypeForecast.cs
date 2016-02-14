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
                return CacheRefFactory.CacheRef.GetLeagueName(BetTypeId);
            }
        }

        public string BetType { get; set; }

        public IEnumerable<BetChoiceForecast> BetChoices { get; set; }
    }
}