using System;

namespace Unseal.Models.Capsules;

public record CapsuleBaseModel(
    Guid? CapsuleTypeId,
    Guid? ReceiverId,
    string Name,
    bool? IsPublic,
    string ContentType,
    string? TextContext,
    string? FileUrl,
    string? FileName,
    DateTime RevealDate
);