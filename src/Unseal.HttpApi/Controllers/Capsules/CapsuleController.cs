using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Unseal.Dtos.Capsules;
using Unseal.Filtering.Capsules;
using Unseal.Permissions.Capsules;
using Unseal.Services.Capsules;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace Unseal.Controllers.Capsules;

[Authorize]
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
    [Authorize(CapsulePermissions.Create)]
    public async Task<bool> CreateAsync(
        [FromForm]CapsuleCreateDto capsuleCreateDto,
        CancellationToken cancellationToken = default
    ) => await CapsuleAppService
        .CreateAsync(capsuleCreateDto, cancellationToken);

    /// <summary>
    /// Used to mark capsules as viewed.
    /// </summary>
    /// <param name="capsuleIds"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("mark-as-viewed")]
    [Authorize(CapsulePermissions.Default)]
    public async Task<bool> MarkAsViewedAsync(
        List<Guid> capsuleIds,
        CancellationToken cancellationToken = default
    ) => await CapsuleAppService.MarkAsViewedAsync(
        capsuleIds,
        cancellationToken
    );
    
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
    [Authorize(CapsulePermissions.Default)]
    public async Task<PagedResultDto<CapsuleDto>> GetFilteredListAsync(
        [FromQuery]CapsuleFilters capsuleFilters,
        bool isAll = true,
        CancellationToken cancellationToken = default
    ) => await CapsuleAppService
        .GetFilteredListAsync(
            capsuleFilters,
            isAll,
            cancellationToken
        );
    
    /// <summary>
    /// Use to feed explore.
    /// </summary>
    /// <param name="capsuleFilters"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("feed")]
    [Authorize(CapsulePermissions.Default)]
    public async Task<PagedResultDto<CapsuleDto>> GetFeedAsync(
        [FromQuery]CapsuleFilters capsuleFilters,
        CancellationToken cancellationToken = default
    ) => await CapsuleAppService
        .GetExploreFeedAsync(
            capsuleFilters,
            cancellationToken
        );
    
    /// <summary>
    /// Use to capsule detail.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [Authorize(CapsulePermissions.Default)]
    public async Task<CapsuleDetailDto> GetDetailAsync(
        Guid id,
        CancellationToken cancellationToken=default
    ) => await CapsuleAppService.GetDetailAsync(id, cancellationToken);

    /// <summary>
    /// Use to get base64 qr code of the capsule
    /// </summary>
    /// <param name="capsuleId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("{capsuleId}/qr-code")]
    [Authorize(CapsulePermissions.Default)]
    public async Task<string> GetQrCodeAsync(
        Guid capsuleId,
        CancellationToken cancellationToken = default
    ) => await CapsuleAppService
        .GetQrCodeAsync(capsuleId, cancellationToken);
    
    /// <summary>
    /// Use to like a capsule.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{id}/like")]
    [Authorize(CapsulePermissions.Default)]
    public async Task<bool> LikeAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) => await CapsuleAppService
        .LikeAsync(id,cancellationToken);
    
    /// <summary>
    /// Use to undo likes on a capsule.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{id}/unlike")]
    [Authorize(CapsulePermissions.Default)]
    public async Task<bool> UnLikeAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) => await CapsuleAppService
        .UnLikeAsync(id,cancellationToken);
    
    /// <summary>
    /// Use to leave comments on capsules.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="comment"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("{id}/comment")]
    [Authorize(CapsulePermissions.Default)]
    public async Task<bool> CommentAsync(
        Guid id,
        string comment,
        CancellationToken cancellationToken = default
    ) => await CapsuleAppService
        .CommentAsync(id, comment, cancellationToken);

    /// <summary>
    /// Use to archive or unarchive capsules.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="isActive"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPatch("{id}/set-activity")]
    public async Task<bool> SetActivityAsync(
        Guid id,
        bool isActive = true,
        CancellationToken cancellationToken = default
    ) => await  CapsuleAppService.SetActivityAsync(
        id,
        isActive,
        cancellationToken
    );
    
    /// <summary>
    /// Use to delete capsule.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [Authorize(CapsulePermissions.Delete)]
    public async Task<bool> DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) => await CapsuleAppService.DeleteAsync(id, cancellationToken);
    
    /// <summary>
    /// Use to delete comments from a capsule.
    /// </summary>
    /// <param name="commentId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpDelete("{commentId}/uncomment")]
    [Authorize(CapsulePermissions.Default)]
    public async Task<bool> UnCommentAsync(
        Guid commentId,
        CancellationToken cancellationToken = default
    ) => await CapsuleAppService
        .UnCommentAsync(commentId,cancellationToken);
}