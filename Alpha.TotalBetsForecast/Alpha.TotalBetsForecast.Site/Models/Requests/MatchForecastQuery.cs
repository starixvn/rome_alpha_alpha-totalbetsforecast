namespace Alpha.TotalBetsForecast.Site.Models.Requests
{
    using MediatR;
    using Sunplus.Agent.Model.Entities;

    public class MatchForecastQuery : IRequest<MatchForecast>
    {
        public int CustId
        {
            get
            {
                return UserSession.Current.CustId;
            }
        }

        public int MatchId { get; set; }

        public bool IsLive { get; set; }

        public short LiveHomeScore { get; set; }

        public short LiveAwayScore { get; set; }
    }
}