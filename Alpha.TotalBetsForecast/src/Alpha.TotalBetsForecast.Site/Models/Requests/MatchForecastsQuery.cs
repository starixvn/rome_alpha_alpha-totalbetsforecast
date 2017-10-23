namespace Alpha.TotalBetsForecast.Site.Models.Requests
{
    using System.Collections.Generic;
    using MediatR;
    using Sunplus.Agent.Model.Entities;
    using Views;

    public class MatchForecastsQuery : IRequest<IEnumerable<MatchForecastViewModel>>
    {
        public MatchForecastsQuery(IUserSession user)
        {
            CustId = user.CustId;
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

        public string MatchIds { get; set; }

        public bool IsLive { get; set; }
    }
}