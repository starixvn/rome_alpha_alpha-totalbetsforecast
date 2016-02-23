namespace Alpha.TotalBetsForecast.Site.Models.Requests
{
    using MediatR;
    using Sunplus.Agent.Model.Entities;
    using Views;

    public class MatchForecastQuery : IRequest<MatchForecastViewModel>
    {
        public MatchForecastQuery()
        {
            CustId = UserSession.Current.CustId;
        }

        internal MatchForecastQuery(int custId)
        {
            custId = custId;
        }

        public int CustId
        {
            get;
            private set;
        }

        public int MatchId { get; set; }

        public bool IsLive { get; set; }

        public short LiveHomeScore { get; set; }

        public short LiveAwayScore { get; set; }
    }
}