namespace Alpha.TotalBetsForecast.Services
{
    using System.Collections.Generic;
    using Alpha.TotalBetsForecast.Criteria;
    using Alpha.TotalBetsForecast.Models;
    using Fanex.Data.Repository;
    using System.Linq;

    public interface BullfightingService
    {
        IEnumerable<BullfightingModel> GetMatchs(int custId, UserLevel userLevel);
    }

    public class BullfightingServiceImpl : BullfightingService
    {
        private readonly IDynamicRepository repository;

        public BullfightingServiceImpl(IDynamicRepository dynamicRepository)
        {
            repository = dynamicRepository;
        }

        public IEnumerable<BullfightingModel> GetMatchs(int custId, UserLevel userLevel)
            => repository.Fetch<BullfightingModel>(new GetBullfightingMatchCriteria(custId, userLevel));
    }
}
