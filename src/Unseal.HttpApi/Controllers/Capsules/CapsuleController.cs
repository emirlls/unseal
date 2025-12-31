using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Unseal.Dtos.Capsules;
using Unseal.Filtering.Capsules;
using Unseal.Services.Capsules;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace Unseal.Controllers.Capsules;

[ApiController]
[Route("api/capsule")]
public class CapsuleController : UnsealController
{
    private readonly IAbpLazyServiceProvider _abpLazyServiceProvider;

    public CapsuleController(IAbpLazyServiceProvider abpLazyServiceProvider)
    {
        _abpLazyServiceProvider = abpLazyServiceProvider;
    }

    private ICapsuleAppService CapsuleAppService =>
        _abpLazyServiceProvider.LazyGetRequiredService<ICapsuleAppService>();

    /// <summary>
    /// Use to create capsule.
    /// </summary>
    /// <param name="capsuleCreateDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<bool> CreateAsync(CapsuleCreateDto capsuleCreateDto,
        CancellationToken cancellationToken = default)
    {
        return await CapsuleAppService.CreateAsync(capsuleCreateDto, cancellationToken);
    }

    /// <summary>
    /// Use to paged capsule list.
    /// To list own capsule, isAll must be false.
    /// To list all capsule, isAll must be true.
    /// </summary>
    /// <param name="capsuleFilters"></param>
    /// <param name="isAll"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<PagedResultDto<CapsuleDto>> GetFilteredListAsync(
        [FromQuery]CapsuleFilters capsuleFilters,
        bool isAll = true,
        CancellationToken cancellationToken = default
    )
    {
        return await CapsuleAppService
            .GetFilteredListAsync(capsuleFilters,isAll, cancellationToken);
    }

    /// <summary>
    /// Use to get base64 qr code of the capsule
    /// </summary>
    /// <param name="capsuleId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{capsuleId}/qr-code")]
    public async Task<string> GetQrCodeAsync(
        Guid capsuleId,
        CancellationToken cancellationToken = default
    )
    {
        return await CapsuleAppService.GetQrCodeAsync(capsuleId, cancellationToken);
    }
}