namespace Alpha.TotalBetsForecast.Site.Models.Requests
{
    using Alpha.TotalBetsForecast.Site.Models.Views;
    using MediatR;

    public class IndexQuery : IRequest<IndexViewModel>
    {
        public IndexQuery()
        {
            IsLive = true;
        }

        public bool IsLive { get; set; }
    }
}