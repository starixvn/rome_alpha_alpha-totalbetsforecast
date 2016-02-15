namespace Alpha.TotalBetsForecast.Site.Models
{
    using System.Collections.Generic;

    public class BetChoiceForecast
    {
        public int BetCount { get; set; }

        public decimal Turnover { get; set; }

        public string BetChoice { get; set; }

        public string HandicapValue { get; set; }

        public IEnumerable<string> WinLossColumnNames { get; set; }

        public IEnumerable<decimal> WinLossValues { get; set; }
    }
}