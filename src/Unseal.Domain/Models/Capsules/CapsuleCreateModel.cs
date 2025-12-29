using System;

namespace Unseal.Models.Capsules;

public record CapsuleCreateModel(
    Guid? CapsuleTypeId,
    Guid? ReceiverId,
    string Name,
    bool? IsPublic,
    string ContentType,
    string? TextContext,
    string? FileUrl,
    string? FileName,
    DateTime RevealDate) :
    CapsuleBaseModel(CapsuleTypeId, ReceiverId, Name, IsPublic, ContentType, TextContext,
        FileUrl, FileName, RevealDate);