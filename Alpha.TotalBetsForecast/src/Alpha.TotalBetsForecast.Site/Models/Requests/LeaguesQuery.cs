namespace Alpha.TotalBetsForecast.Site.Models.Requests
{
    using System.Collections.Generic;
    using MediatR;
    using Sunplus.Agent.Model.BL.Definitions;

    public class LeaguesQuery : IRequest<IEnumerable<League>>
    {
        public LeaguesQuery()
        {

        }

        public LeaguesQuery(bool isLive) : this(isLive, AppSettings.LocalTest)
        {

        }
        public LeaguesQuery(bool isLive, bool isLocalTest)
        {
            IsLive = isLive;
            IsLocalTest = isLocalTest;
        }

        public bool IsLive { get; set; }

        public bool IsLocalTest { get; set; }
    }
}