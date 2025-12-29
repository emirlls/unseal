using System;
using System.Collections.Generic;
using Volo.Abp.Content;

namespace Unseal.Models.Users;

public record GroupCreateModel(
    List<Guid> UserIds,
    string Name,
    string Description,
    IRemoteStreamContent? StreamContent = null
    );