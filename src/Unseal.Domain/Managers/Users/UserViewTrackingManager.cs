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

    public List<UserViewTracking> Create(
        List<Guid> externalIds, 
        Guid? userViewTrackingTypeId,
        Guid userId)
    {
        var entities = new List<UserViewTracking>();
        var dateTimeNow = DateTime.Now;
        foreach (var externalId in externalIds)
        {
            var entity = new UserViewTracking(
                GuidGenerator.Create(),
                userId,
                externalId,
                userViewTrackingTypeId,
                dateTimeNow
            );
            entities.Add(entity);
        }

        return entities;
    }
}