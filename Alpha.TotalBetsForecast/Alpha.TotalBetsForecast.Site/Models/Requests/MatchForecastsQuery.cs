namespace Alpha.TotalBetsForecast.Site.Models.Requests
{
    using MediatR;
    using Sunplus.Agent.Model.Entities;
    using Views;

    public class MatchForecastsQuery : IRequest<MatchForecastsViewModel>
    {
        public MatchForecastsQuery()
        {
            CustId = UserSession.Current.CustId;
        }

        internal MatchForecastsQuery(int custId)
        {
            CustId = custId;
        }

        public int CustId
        {
            get;
            private set;
        }

        public int LeagueId { get; set; }

        public string MatchIds { get; set; }

        public bool IsLive { get; set; }
    }
}