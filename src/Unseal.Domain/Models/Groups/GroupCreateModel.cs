using System;
using System.Collections.Generic;
using Volo.Abp.Content;

namespace Unseal.Models.Groups;

public record GroupCreateModel(
    List<Guid> UserIds,
    string Name,
    string Description,
    IRemoteStreamContent? StreamContent = null
    );