using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unseal.Constants;
using Unseal.Etos;
using Unseal.Interfaces.Managers.Users;
using Unseal.Models.ElasticSearch;
using Unseal.Repositories.Users;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;

namespace Unseal.Handlers;

public class UserViewTrackingEventHandler :
    IDistributedEventHandler<UserViewTrackingEto>, ITransientDependency

{
    private readonly IUserViewTrackingManager _userViewTrackingManager;
    private readonly IUserViewTrackingRepository _userViewTrackingRepository;
    private readonly IEsUserViewTrackingRepository _esUserViewTrackingRepository;
    private readonly IUnitOfWorkManager _unitOfWorkManager;

    public UserViewTrackingEventHandler(
        IUnitOfWorkManager unitOfWorkManager, 
        IUserViewTrackingManager userViewTrackingManager,
        IUserViewTrackingRepository userViewTrackingRepository,
        IEsUserViewTrackingRepository esUserViewTrackingRepository)
    {
        _unitOfWorkManager = unitOfWorkManager;
        _userViewTrackingManager = userViewTrackingManager;
        _userViewTrackingRepository = userViewTrackingRepository;
        _esUserViewTrackingRepository = esUserViewTrackingRepository;
    }

    public async Task HandleEventAsync(UserViewTrackingEto eventData)
    {
        using (var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true))
        {

            var capsuleIds = eventData.ExternalIds;
            var alreadyViewedCapsuleIds = (await _userViewTrackingManager.TryGetQueryableAsync(q => q
                    .Where(x => x.UserId.Equals(eventData.UserId) && capsuleIds.Contains(x.ExternalId))))?
                .Select(x => x.ExternalId)
                .ToHashSet();

            capsuleIds = capsuleIds
                .WhereIf(!alreadyViewedCapsuleIds.IsNullOrEmpty(),
                    x => !alreadyViewedCapsuleIds!.Contains(x))
                .ToList();

            var userViewTrackings = _userViewTrackingManager
                .Create(
                    capsuleIds, 
                    eventData.UserViewTrackingTypeId,
                    eventData.UserId
                );

            await _userViewTrackingRepository.BulkInsertAsync(
                userViewTrackings);

            var userViewTrackingElasticModel =
                userViewTrackings
                    .Select(x => new UserViewTrackingElasticModel
                    {
                        Id = x.Id,
                        UserId = x.UserId,
                        ExternalId = x.ExternalId,
                        UserViewTrackingTypeId = x.UserViewTrackingTypeId
                    });

            await _esUserViewTrackingRepository
                .BulkCreateDocumentsAsync(
                    userViewTrackingElasticModel,
                    ElasticSearchConstants.User.UserViewTrackingIndex
                );
            
            await uow.CompleteAsync();
        }
    }
}