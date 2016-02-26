namespace Alpha.TotalBetsForecast.Site.Helpers
{
    using System.Collections.Generic;
    using Sunplus.Agent.Model.BL.Definitions;

    internal class BetTypeComparer : IComparer<int>
    {
        private static IDictionary<int, int> _betTypeOrders = new Dictionary<int, int>
        {
            { (int)BetType.Handicap, 1 },
            { (int)BetType._1st_Handicap, 2 },
            { (int)BetType.Over_Under, 3 },
            { (int)BetType._1st_Over_Under, 4 },
            { (int)BetType.FT_1X2, 5 },
            { (int)BetType.FirstHalf_1X2, 6 }
        };

        public int Compare(int x, int y)
        {
            if (_betTypeOrders.ContainsKey(x) && _betTypeOrders.ContainsKey(y))
            {
                return _betTypeOrders[x] - _betTypeOrders[y];
            }

            return 0;
        }
    }
}