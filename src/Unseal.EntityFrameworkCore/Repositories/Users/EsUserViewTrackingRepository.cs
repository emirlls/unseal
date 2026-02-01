using System;
using Unseal.Models.ElasticSearch;
using Unseal.Repositories.Base;

namespace Unseal.Repositories.Users;

public class EsUserViewTrackingRepository : ElasticSearchRepository<UserViewTrackingElasticModel, Guid>,
    IEsUserViewTrackingRepository
{
    public EsUserViewTrackingRepository(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }
}