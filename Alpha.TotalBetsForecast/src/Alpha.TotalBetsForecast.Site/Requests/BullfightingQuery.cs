namespace Alpha.TotalBetsForecast.Site.Requests
{
    using Alpha.TotalBetsForecast.Site.ViewModels;
    using MediatR;
    using Sunplus.Agent.Model.Entities;
    using TotalBetsForecast.Enumerations;
    using TotalBetsForecast.Models;

    public class BullfightingQuery : IRequest<BullfightingViewModel>
    {
        public BullfightingQuery(IUserSession user)
        {
            CustId = user.CustId;
            CustLevelId = user.RoleId;
        }

        public int CustId { get; }

        public byte CustLevelId { get; }

        public UserLevel CustLevel => Enumeration.FromValue<UserLevel>(CustLevelId);
    }
}