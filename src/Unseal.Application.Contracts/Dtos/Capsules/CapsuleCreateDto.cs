using System;
using Volo.Abp.Content;

namespace Unseal.Dtos.Capsules;

public record CapsuleCreateDto(
    Guid? CapsuleTypeId,
    Guid? ReceiverId,
    string Name,
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
        RevealDate,
        StreamContent
    );