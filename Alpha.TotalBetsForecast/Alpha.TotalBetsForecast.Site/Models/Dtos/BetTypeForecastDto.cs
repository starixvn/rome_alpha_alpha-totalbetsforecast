namespace Alpha.TotalBetsForecast.Site.Models.Dtos
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    public class BetTypeForecastDto
    {
        public int BetTypeId { get; set; }

        public string BetType { get; set; }

        public int BetCount { get; set; }

        public decimal Turnover { get; set; }

        public string BetChoice { get; set; }

        public string FirstColumnName { get; set; }

        public decimal WinLossFirstColumn { get; set; }

        public string SecondColumnName { get; set; }

        public decimal WinLossSecondColumn { get; set; }

        public string ThirdColumnName { get; set; }

        public decimal WinLossThirdColumn { get; set; }
    }
}