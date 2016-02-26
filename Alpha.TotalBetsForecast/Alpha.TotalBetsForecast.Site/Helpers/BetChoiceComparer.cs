namespace Alpha.TotalBetsForecast.Site.Helpers
{
    using System.Collections.Generic;

    internal class BetChoiceComparer : IComparer<string>
    {
        private static IDictionary<string, int> _betChoiceOrders = new Dictionary<string, int>
        {
            { "h", 1 },
            { "a", 2 },
            { "1", 3 },
            { "2", 4 },
            { "x", 5 }
        };

        public int Compare(string x, string y)
        {
            if (!string.IsNullOrEmpty(x) 
                && !string.IsNullOrEmpty(y)
                && _betChoiceOrders.ContainsKey(x.ToLower()) 
                && _betChoiceOrders.ContainsKey(y.ToLower()))
            {
                return _betChoiceOrders[x.ToLower()] - _betChoiceOrders[y.ToLower()];
            }

            return 0;
        }
    }
}