using System;
using Riok.Mapperly.Abstractions;
using Unseal.Constants;
using Unseal.Dtos.Capsules;
using Unseal.Entities.Lookups;
using Unseal.Enums;
using Unseal.Extensions;
using Unseal.Models.Capsules;
using Volo.Abp.DependencyInjection;

namespace Unseal.Profiles.Capsules;

[Mapper]
public partial class CapsuleMapper : ITransientDependency
{
    public string? ResolveType(CapsuleType? capsuleType)
    {
        if (capsuleType is null)
        {
            return null;
        }

        return ((CapsuleTypes)capsuleType?.Code!).GetDescription();
    }

    public CapsuleCreateModel MaptoModel(CapsuleCreateDto capsuleCreateDto)
    {
        var capsuleTypeId = capsuleCreateDto.ReceiverId.HasValue
            ? Guid.Parse(LookupSeederConstants.CapsuleTypesConstants.Personal.Id)
            : Guid.Parse(LookupSeederConstants.CapsuleTypesConstants.Public.Id);
        
        return new CapsuleCreateModel
        {
            CapsuleTypeId = capsuleTypeId,
            ReceiverId = capsuleCreateDto.ReceiverId,
            Name = capsuleCreateDto.Name,
            ContentType = capsuleCreateDto.StreamContent.ContentType,
            TextContext = capsuleCreateDto.TextContext,
            FileUrl = string.Empty, // will be set after upload file.
            FileName = capsuleCreateDto.StreamContent.FileName,
            GeoJson = capsuleCreateDto.GeoJson,
            RevealDate = capsuleCreateDto.RevealDate
        };

    }
}