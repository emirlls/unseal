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
                LookupSeederConstants.NotificationEventTypesConstants.ConfirmChangeMail.Code),
            
            new NotificationEventType(Guid.Parse(LookupSeederConstants.NotificationEventTypesConstants.UserActivation.Id),
                LookupSeederConstants.NotificationEventTypesConstants.UserActivation.Name,
                LookupSeederConstants.NotificationEventTypesConstants.UserActivation.Code),
            
            new NotificationEventType(Guid.Parse(LookupSeederConstants.NotificationEventTypesConstants.PasswordReset.Id),
                LookupSeederConstants.NotificationEventTypesConstants.PasswordReset.Name,
                LookupSeederConstants.NotificationEventTypesConstants.PasswordReset.Code)
        );
        #endregion

        #region ChatTypes

        builder.Entity<ChatType>().HasData(
            new ChatType(
                Guid.Parse(LookupSeederConstants.ChatTypesConstants.Directly.Id),
                LookupSeederConstants.ChatTypesConstants.Directly.Name,
                LookupSeederConstants.ChatTypesConstants.Directly.Code),
            
            new ChatType(Guid.Parse(LookupSeederConstants.ChatTypesConstants.Group.Id),
                LookupSeederConstants.ChatTypesConstants.Group.Name,
                LookupSeederConstants.ChatTypesConstants.Group.Code)
        );
        #endregion

        #region FollowStatuses

        builder.Entity<UserFollowStatus>().HasData(
            new UserFollowStatus(
                Guid.Parse(LookupSeederConstants.UserFollowStatusesConstants.Pending.Id),
                LookupSeederConstants.UserFollowStatusesConstants.Pending.Name,
                LookupSeederConstants.UserFollowStatusesConstants.Pending.Code),

            new UserFollowStatus(Guid.Parse(LookupSeederConstants.UserFollowStatusesConstants.Accepted.Id),
                LookupSeederConstants.UserFollowStatusesConstants.Accepted.Name,
                LookupSeederConstants.UserFollowStatusesConstants.Accepted.Code),

            new UserFollowStatus(Guid.Parse(LookupSeederConstants.UserFollowStatusesConstants.Rejected.Id),
                LookupSeederConstants.UserFollowStatusesConstants.Rejected.Name,
                LookupSeederConstants.UserFollowStatusesConstants.Rejected.Code)
        );

        #endregion

        #region UserViewTrackingTypes
        builder.Entity<UserViewTrackingType>().HasData(
            new UserViewTrackingType(
                Guid.Parse(LookupSeederConstants.UserViewTrackingTypesConstants.Capsule.Id),
                LookupSeederConstants.UserViewTrackingTypesConstants.Capsule.Name,
                LookupSeederConstants.UserViewTrackingTypesConstants.Capsule.Code),

            new UserViewTrackingType(Guid.Parse(LookupSeederConstants.UserViewTrackingTypesConstants.UserProfile.Id),
                LookupSeederConstants.UserViewTrackingTypesConstants.UserProfile.Name,
                LookupSeederConstants.UserViewTrackingTypesConstants.UserProfile.Code)

        );
        #endregion
    }
}