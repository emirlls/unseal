using System;
using Volo.Abp.Content;

namespace Unseal.Dtos.Capsules;

public record CapsuleBaseDto(
    Guid? CapsuleTypeId,
    Guid? ReceiverId,
    string Name,
    string TextContext,
    string? GeoJson,
    DateTime RevealDate,
    IRemoteStreamContent StreamContent
);