using System;

namespace Unseal.Models.Capsules;

public record CapsuleBaseModel(
    Guid? CapsuleTypeId,
    Guid? ReceiverId,
    string Name,
    string ContentType,
    string? TextContext,
    string? FileUrl,
    string? FileName,
    string? GeoJson,
    DateTime RevealDate
);