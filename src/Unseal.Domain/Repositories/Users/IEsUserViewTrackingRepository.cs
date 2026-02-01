using System;
using Unseal.Models.ElasticSearch;
using Unseal.Repositories.Base;

namespace Unseal.Repositories.Users;

public interface IEsUserViewTrackingRepository : IElasticSearchRepository<UserViewTrackingElasticModel, Guid>
{
}