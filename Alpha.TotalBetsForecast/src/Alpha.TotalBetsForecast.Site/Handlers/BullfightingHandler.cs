namespace Alpha.TotalBetsForecast.Site.Handlers
{
    using Alpha.TotalBetsForcast.Services;
    using Alpha.TotalBetsForecast.Site.Requests;
    using Alpha.TotalBetsForecast.Site.ViewModel;
    using MediatR;

    public class BullfightingHandler : IRequest<BullfightingViewModel>
    {
        private readonly BullfightingService service;

        public BullfightingHandler(BullfightingService service)
        {
            this.service = service;
        }

        public BullfightingViewModel Handle(BullfightingQuery query)
        {
            var matchs = service.GetMatchs(query.CustId);

            return new BullfightingViewModel(matchs);
        }
    }
}