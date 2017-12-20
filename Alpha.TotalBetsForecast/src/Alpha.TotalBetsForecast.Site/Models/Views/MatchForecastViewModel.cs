namespace Alpha.TotalBetsForecast.Site.Models.Views
{
    using System;
    using System.Collections.Generic;

    public class MatchForecastViewModel
    {
        public int Id { get; set; }

        public bool IsLive { get; set; }

        public string HomeName { get; set; }

        public string AwayName { get; set; }

        public byte LiveHomeScore { get; set; }

        public byte LiveAwayScore { get; set; }

        public bool HasLive { get; set; }

        public DateTime KickOffTime { get; set; }

        public string ShowTime { get; set; }

        public IEnumerable<BetTypeForecast> BetTypeForecasts { get; set; }

        public int TotalBetCount { get; set; }

        public decimal TotalTurnover { get; set; }

        public decimal TotalForecastWinLoss { get; set; }

        public int MatchRowSpan { get; set; }

        public string KickOffTimeString
        {
            get
            {
                return KickOffTime.ToString("hh:mm tt");
            }
        }

        public string KickOffDateString
        {
            get
            {
                return KickOffTime.ToString("MM/dd/yyyy");
            }
        }
    }
}