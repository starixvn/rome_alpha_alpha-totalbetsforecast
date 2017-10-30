namespace Alpha.TotalBetsForecast.Site.Handlers
{
    using Alpha.TotalBetsForecast.Services;
    using Alpha.TotalBetsForecast.Site.Requests;
    using Alpha.TotalBetsForecast.Site.ViewModels;
    using MediatR;

    public class BullfightingHandler : IRequestHandler<BullfightingQuery, BullfightingViewModel>
    {
        private readonly BullfightingService service;

        public BullfightingHandler(BullfightingService service)
        {
            this.service = service;
        }

        public BullfightingViewModel Handle(BullfightingQuery message)
        {
            var matchs = service.GetMatchs(message.CustId, message.CustLevel);

            return new BullfightingViewModel(matchs);
        }
    }
}