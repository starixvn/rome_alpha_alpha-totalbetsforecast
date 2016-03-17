namespace Alpha.TotalBetsForecast.Site.Models.Requests
{
    using MediatR;
    using Sunplus.Agent.Model.Entities;
    using Views;

    public class MatchForecastQuery : IRequest<LeagueForecastViewModel>
    {
        public MatchForecastQuery()
        {
            CustId = UserSession.Current.CustId;
        }

        internal MatchForecastQuery(int custId)
        {
            CustId = custId;
        }

        public int CustId
        {
            get;
            private set;
        }

        public int LeagueId { get; set; }

        public int MatchId { get; set; }
    }
}