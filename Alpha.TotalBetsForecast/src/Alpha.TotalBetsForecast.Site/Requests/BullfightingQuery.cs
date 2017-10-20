namespace Alpha.TotalBetsForecast.Site.Requests
{
    using Alpha.TotalBetsForecast.Site.ViewModel;
    using MediatR;
    using Sunplus.Agent.Model.Entities;

    public class BullfightingQuery : IRequest<BullfightingViewModel>
    {
        public BullfightingQuery(IUserSession user)
        {
            CustId = user.CustId;
        }

        public int CustId { get; }
    }
}