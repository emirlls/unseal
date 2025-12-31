using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unseal.Dtos.Capsules;
using Unseal.Entities.Capsules;
using Unseal.Extensions;
using Unseal.Filtering.Capsules;
using Unseal.Interfaces.Managers.Capsules;
using Unseal.Interfaces.Managers.Users;
using Unseal.Profiles.Capsules;
using Unseal.Repositories.Capsules;
using Volo.Abp.Application.Dtos;

namespace Unseal.Services.Capsules;

public class CapsuleAppService : UnsealAppService, ICapsuleAppService
{
    private CapsuleMapper CapsuleMapper =>
        LazyServiceProvider.LazyGetRequiredService<CapsuleMapper>();

    private ICapsuleManager CapsuleManager =>
        LazyServiceProvider.LazyGetRequiredService<ICapsuleManager>();

    private ICapsuleItemManager CapsuleItemManager =>
        LazyServiceProvider.LazyGetRequiredService<ICapsuleItemManager>();
    
    private ICapsuleItemRepository CapsuleItemRepository =>
        LazyServiceProvider.LazyGetRequiredService<ICapsuleItemRepository>();
    
    private ICapsuleRepository CapsuleRepository =>
        LazyServiceProvider.LazyGetRequiredService<ICapsuleRepository>();
    private IUserInteractionManager UserInteractionManager =>
        LazyServiceProvider.LazyGetRequiredService<IUserInteractionManager>();
    
    public async Task<bool> CreateAsync(CapsuleCreateDto capsuleCreateDto, CancellationToken cancellationToken = default)
    {
        var fileUrl = await LazyServiceProvider.UploadFileAsync(capsuleCreateDto.StreamContent);
        var encryptedFileUrl = LazyServiceProvider.GetEncryptedFileUrlAsync(fileUrl);
        var capsuleCreateModel = CapsuleMapper.MapCapsuleCreateDtoToCapsuleCreateModel(capsuleCreateDto);
        var capsule = CapsuleManager.Create(capsuleCreateModel, CurrentUser.Id!);
        capsuleCreateModel = capsuleCreateModel with { FileUrl = encryptedFileUrl };
        var capsuleItem = CapsuleItemManager.Create(capsuleCreateModel, capsule.Id);
        await CapsuleRepository.InsertAsync(capsule, cancellationToken: cancellationToken);
        await CapsuleItemRepository.InsertAsync(capsuleItem, cancellationToken: cancellationToken);
        return true;
    }

    public async Task<PagedResultDto<CapsuleDto>> GetFilteredListAsync(
        CapsuleFilters capsuleFilters,
        bool isAll = true,
        CancellationToken cancellationToken = default
    )
    {
        capsuleFilters.IsOpened = true;
        Func<IQueryable<Capsule>, IQueryable<Capsule>>? queryBuilder = null;
        
        if (!isAll)
        {
            queryBuilder = capsule => capsule
                .Where(c => c.CreatorId == CurrentUser.Id);
        }
        else
        {
            var usersBlockedCurrentUser = await UserInteractionManager
                .TryGetQueryableAsync(x =>
                        x.Where(c => 
                            c.TargetUserId.Equals(CurrentUser.Id) && c.IsBlocked),
                    cancellationToken: cancellationToken);
            var capsuleCreators = 
                usersBlockedCurrentUser != null && usersBlockedCurrentUser.Any() ? 
                    usersBlockedCurrentUser
                    .Select(x => x.SourceUserId)
                    .ToHashSet() : null;
            queryBuilder = capsule => capsule
                .WhereIf(!capsuleCreators.IsNullOrEmpty(),
                    x => !capsuleCreators.Contains((Guid)x.CreatorId!));
        }
        var capsules = await CapsuleRepository
            .GetDynamicListAsync(capsuleFilters,queryBuilder,true, cancellationToken);
        var count = await CapsuleRepository
            .GetDynamicListCountAsync(capsuleFilters, cancellationToken);
        var dto = CapsuleMapper.MapCapsuleListToCapsuleDtoList(capsules);
        var response = new PagedResultDto<CapsuleDto>
        {
            Items = dto,
            TotalCount = count
        };
        return response;
    }

    public async Task<string> GetQrCodeAsync(
        Guid capsuleId,
        CancellationToken cancellationToken = default
    )
    {
        await CapsuleManager.TryGetByAsync(x => 
            x.Id.Equals(capsuleId), throwIfNull: true, 
            cancellationToken: cancellationToken);
        var base64QrCode = await LazyServiceProvider.GenerateQrCodeAsync(capsuleId);
        return base64QrCode;
    }
}