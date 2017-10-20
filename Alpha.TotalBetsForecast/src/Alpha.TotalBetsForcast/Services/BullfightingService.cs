namespace Alpha.TotalBetsForcast.Services
{
    using System.Collections.Generic;
    using Alpha.TotalBetsForcast.Model;
    using Fanex.Data.Repository;
    using Alpha.TotalBetsForcast.Criteria;

    public interface BullfightingService
    {
        IEnumerable<BullfightingModel> GetMatchs(int custId);
    }

    public class BullfightingImpl : BullfightingService
    {
        private readonly IDynamicRepository repository;

        public BullfightingImpl(IDynamicRepository dynamicRepository)
        {
            repository = dynamicRepository;
        }

        public IEnumerable<BullfightingModel> GetMatchs(int custId)
        {
            return repository.Fetch<BullfightingModel>(new GetBullfightingMatchCriteria(custId));
        }
    }
}
