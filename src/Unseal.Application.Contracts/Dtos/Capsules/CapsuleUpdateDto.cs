using System;
using Volo.Abp.Content;

namespace Unseal.Dtos.Capsules;

public record CapsuleUpdateDto(
    Guid? CapsuleTypeId,
    Guid? ReceiverId,
    string Name,
    bool? IsPublic,
    string TextContext,
    string? GeoJson,
    DateTime RevealDate,
    IRemoteStreamContent StreamContent) :
    CapsuleBaseDto(
        CapsuleTypeId,
        ReceiverId,
        Name,
        TextContext,
        GeoJson,
        IsPublic,
        RevealDate,
        StreamContent
    );