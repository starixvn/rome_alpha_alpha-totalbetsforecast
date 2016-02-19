namespace Alpha.TotalBetsForecast.Site.Helpers
{
    using System.Collections.Generic;

    public class BetChoiceComparer : IComparer<string>
    {
        private static IDictionary<string, int> _betChoiceOrders = new Dictionary<string, int>
        {
            { "h", 1 },
            { "a", 2 },
            { "1", 3 },
            { "x", 4 },
            { "2", 5 }
        };

        public int Compare(string x, string y)
        {
            if (_betChoiceOrders.ContainsKey(x.ToLower()) && _betChoiceOrders.ContainsKey(y.ToLower()))
            {
                return _betChoiceOrders[x.ToLower()] - _betChoiceOrders[y.ToLower()];
            }

            return 0;
        }
    }
}