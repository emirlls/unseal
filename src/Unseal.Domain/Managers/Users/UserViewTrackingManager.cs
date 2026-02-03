using System;
using System.Collections.Generic;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Entities.Users;
using Unseal.Interfaces.Managers.Users;
using Unseal.Localization;
using Unseal.Repositories.Users;

namespace Unseal.Managers.Users;

public class UserViewTrackingManager : BaseDomainService<UserViewTracking>, IUserViewTrackingManager
{
    public UserViewTrackingManager(
        IUserViewTrackingRepository baseRepository,
        IStringLocalizer<UnsealResource> stringLocalizer
        ) : base(
        baseRepository, 
        stringLocalizer,
        ExceptionCodes.UserViewTracking.NotFound,
        ExceptionCodes.UserViewTracking.AlreadyExists
    )
    {
    }

    public List<UserViewTracking> Create(List<Guid> capsuleIds, Guid userId)
    {
        var entities = new List<UserViewTracking>();
        var dateTimeNow = DateTime.Now;
        foreach (var capsuleId in capsuleIds)
        {
            var entity = new UserViewTracking(
                GuidGenerator.Create(),
                userId,
                capsuleId,
                dateTimeNow
            );
            entities.Add(entity);
        }

        return entities;
    }
}