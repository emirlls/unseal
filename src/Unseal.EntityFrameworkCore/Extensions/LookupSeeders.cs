using System;
using Microsoft.EntityFrameworkCore;
using Unseal.Constants;
using Unseal.Entities.Lookups;
using Unseal.Entities.Notifications;

namespace Unseal.Extensions;

public static class LookupSeeders
{
    public static void LookupSeeder(this ModelBuilder builder)
    {
        #region CapsuleTypes

        builder.Entity<CapsuleType>().HasData(
            new CapsuleType(
                Guid.Parse(LookupSeederConstants.CapsuleTypesConstants.Personal.Id),
                LookupSeederConstants.CapsuleTypesConstants.Personal.Name,
                LookupSeederConstants.CapsuleTypesConstants.Personal.Code),
            
            new CapsuleType(Guid.Parse(LookupSeederConstants.CapsuleTypesConstants.DirectMessage.Id),
                LookupSeederConstants.CapsuleTypesConstants.DirectMessage.Name,
                LookupSeederConstants.CapsuleTypesConstants.DirectMessage.Code),
            
            new CapsuleType(Guid.Parse(LookupSeederConstants.CapsuleTypesConstants.PublicBroadcast.Id),
                LookupSeederConstants.CapsuleTypesConstants.PublicBroadcast.Name,
                LookupSeederConstants.CapsuleTypesConstants.PublicBroadcast.Code),
            
            new CapsuleType(Guid.Parse(LookupSeederConstants.CapsuleTypesConstants.Collaborative.Id),
                LookupSeederConstants.CapsuleTypesConstants.Collaborative.Name,
                LookupSeederConstants.CapsuleTypesConstants.Collaborative.Code),
            
            new CapsuleType(Guid.Parse(LookupSeederConstants.CapsuleTypesConstants.Public.Id),
                LookupSeederConstants.CapsuleTypesConstants.Public.Name,
                LookupSeederConstants.CapsuleTypesConstants.Public.Code)
        );
        #endregion
        
        #region NotificationEventTypes

        builder.Entity<NotificationEventType>().HasData(
            new NotificationEventType(
                Guid.Parse(LookupSeederConstants.NotificationEventTypesConstants.UserRegister.Id),
                LookupSeederConstants.NotificationEventTypesConstants.UserRegister.Name,
                LookupSeederConstants.NotificationEventTypesConstants.UserRegister.Code),
            
            new NotificationEventType(Guid.Parse(LookupSeederConstants.NotificationEventTypesConstants.UserDelete.Id),
                LookupSeederConstants.NotificationEventTypesConstants.UserDelete.Name,
                LookupSeederConstants.NotificationEventTypesConstants.UserDelete.Code),
            
            new NotificationEventType(Guid.Parse(LookupSeederConstants.NotificationEventTypesConstants.ConfirmChangeMail.Id),
                LookupSeederConstants.NotificationEventTypesConstants.ConfirmChangeMail.Name,
                LookupSeederConstants.NotificationEventTypesConstants.ConfirmChangeMail.Code)
        );
        #endregion
    }
}