using System;
using System.Threading;
using System.Threading.Tasks;
using Unseal.Dtos.Capsules;
using Unseal.Extensions;
using Unseal.Interfaces.Managers.Capsules;
using Unseal.Profiles.Capsules;
using Unseal.Repositories.Capsules;

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
    private IServiceProvider ServiceProvider =>
        LazyServiceProvider.LazyGetRequiredService<IServiceProvider>();

    public async Task<bool> CreateAsync(CapsuleCreateDto capsuleCreateDto, CancellationToken cancellationToken = default)
    {
        var fileUrl = await ServiceProvider.UploadFileAsync(capsuleCreateDto.StreamContent);
        var capsuleCreateModel = CapsuleMapper.MapCapsuleCreateDtoToCapsuleCreateModel(capsuleCreateDto);
        var capsule = CapsuleManager.Create(capsuleCreateModel, CurrentUser.Id!);
        capsuleCreateModel = capsuleCreateModel with { FileUrl = fileUrl };
        var capsuleItem = CapsuleItemManager.Create(capsuleCreateModel, capsule.Id);
        await CapsuleRepository.InsertAsync(capsule, cancellationToken: cancellationToken);
        await CapsuleItemRepository.InsertAsync(capsuleItem, cancellationToken: cancellationToken);
        return true;
    }
}