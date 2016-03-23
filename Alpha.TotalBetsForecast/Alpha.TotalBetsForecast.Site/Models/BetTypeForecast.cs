namespace Alpha.TotalBetsForecast.Site.Models
{
    using System.Collections.Generic;
    using App_GlobalResources;

    public class BetTypeForecast
    {
        public int BetTypeId { get; set; }

        public string BetTypeName
        {
            get
            {
                return Resources.ResourceManager.GetString($"BetType_{BetTypeId}");
            }
        }

        public int BetCount { get; set; }

        public decimal Turnover { get; set; }

        public IEnumerable<string> WinLossColumnNames { get; set; }

        public IEnumerable<decimal> WinLossValues { get; set; }

        public bool IsHandicap
        {
            get
            {
                return BetTypeId == (int)Sunplus.Agent.Model.BL.Definitions.BetType.Handicap
                            || BetTypeId == (int)Sunplus.Agent.Model.BL.Definitions.BetType._1st_Handicap;
            }
        }

        public bool IsOverUnder
        {
            get
            {
                return BetTypeId == (int)Sunplus.Agent.Model.BL.Definitions.BetType.Over_Under
                            || BetTypeId == (int)Sunplus.Agent.Model.BL.Definitions.BetType._1st_Over_Under;
            }
        }

        public bool IsOneXTwo
        {
            get
            {
                return BetTypeId == (int)Sunplus.Agent.Model.BL.Definitions.BetType.FT_1X2
                            || BetTypeId == (int)Sunplus.Agent.Model.BL.Definitions.BetType.FirstHalf_1X2;
            }
        }

        public int MainWinLossIndex { get; set; }
    }
}