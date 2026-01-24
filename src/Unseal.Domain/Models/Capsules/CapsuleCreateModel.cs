using System;

namespace Unseal.Models.Capsules;

public record CapsuleCreateModel(
    Guid? CapsuleTypeId,
    Guid? ReceiverId,
    string Name,
    string ContentType,
    string? TextContext,
    string? FileUrl,
    string? FileName,
    string? GeoJson,
    DateTime RevealDate) :
    CapsuleBaseModel(CapsuleTypeId, ReceiverId, Name, ContentType, TextContext,
        FileUrl, FileName, GeoJson, RevealDate);