namespace Alpha.TotalBetsForecast.Site.Models.Requests
{
    using MediatR;
    using Sunplus.Agent.Model.Entities;
    using Views;

    public class MatchForecastQuery : IRequest<MatchForecastViewModel>
    {
        public int CustId
        {
            get
            {
                //return UserSession.Current.CustId;
                return 14649182;
            }
        }

        public int MatchId { get; set; }

        public bool IsLive { get; set; }

        public short LiveHomeScore { get; set; }

        public short LiveAwayScore { get; set; }
    }
}