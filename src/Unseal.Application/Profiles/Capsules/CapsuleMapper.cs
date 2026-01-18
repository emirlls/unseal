using System.Collections.Generic;
using Riok.Mapperly.Abstractions;
using Unseal.Dtos.Capsules;
using Unseal.Entities.Capsules;
using Unseal.Entities.Lookups;
using Unseal.Enums;
using Unseal.Extensions;
using Unseal.Models.Capsules;
using Volo.Abp.DependencyInjection;

namespace Unseal.Profiles.Capsules;

[Mapper]
public partial class CapsuleMapper : ITransientDependency
{
    public partial List<CapsuleDto> MapCapsuleListToCapsuleDtoList(List<Capsule> capsules);
    [MapProperty(nameof(Capsule.CapsuleType), nameof(CapsuleDto.Type), Use = nameof(ResolveType))]
    public partial CapsuleDto MapCapsuleToCapsuleDto(Capsule capsule);
    
    protected string? ResolveType(CapsuleType? capsuleType)
    {
        if (capsuleType is null) return null;
        return ((CapsuleTypes)capsuleType?.Code!).GetDescription();
    }
    public CapsuleCreateModel MapCapsuleCreateDtoToCapsuleCreateModel(CapsuleCreateDto capsuleCreateDto)
    {
        return new CapsuleCreateModel(
            capsuleCreateDto.CapsuleTypeId,
            capsuleCreateDto.ReceiverId,
            capsuleCreateDto.Name,
            capsuleCreateDto.IsPublic,
            capsuleCreateDto.StreamContent.ContentType,
            capsuleCreateDto.TextContext,
            string.Empty, // will be set after upload file.
            capsuleCreateDto.StreamContent.FileName,
            capsuleCreateDto.GeoJson,
            capsuleCreateDto.RevealDate);
    }
}