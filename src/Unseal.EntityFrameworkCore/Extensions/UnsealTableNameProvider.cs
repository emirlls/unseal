using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unseal.Constants;
using Unseal.Entities.Capsules;
using Unseal.Entities.Groups;
using Unseal.Entities.Lookups;
using Unseal.Entities.Messages;
using Unseal.Entities.Notifications;
using Unseal.Entities.Users;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.OpenIddict;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace Unseal.Extensions;

public static class UnsealTableNameProvider
{
    public static string GetTableName<T>(this EntityTypeBuilder<T> entityTypeBuilder) 
        where T : class
    {
        TableNames!.TryGetValue(typeof(T).Name, out var name);
        return name!;
    }

    private static readonly Dictionary<string, string>? TableNames = new()
    {
        {nameof(Capsule),"Capsules"},
        {nameof(CapsuleItem),"CapsuleItems"},
        {nameof(CapsuleType),"CapsuleTypes"},
        {nameof(Notification),"Notifications"},
        {nameof(NotificationEventType),"NotificationEventTypes"},
        {nameof(NotificationTemplate),"NotificationTemplates"},
        {nameof(UserProfile),"UserProfiles"},
        {nameof(UserFollower),"UserFollowers"},
        {nameof(UserFollowStatus),"UserFollowStatuses"},
        {nameof(UserViewTracking),"UserViewTrackings"},
        {nameof(Group),"Groups"},
        {nameof(GroupMember),"GroupMembers"},
        {nameof(UserInteraction),"UserInteractions"},
        {nameof(CapsuleLike),"CapsuleLikes"},
        {nameof(CapsuleComment),"CapsuleComments"},
        {nameof(CapsuleMapFeature),"CapsuleMapFeatures"},
        {nameof(ChatMessage),"ChatMessages"},
        {nameof(ChatType),"ChatTypes"}
    };
    
    public static void SetAbpTablePrefix(this ModelBuilder builder)
    {
        //Identity
        AbpIdentityDbProperties.DbTablePrefix = string.Empty;
        AbpIdentityDbProperties.DbSchema = DatabaseConstants.IdentitySchema;
        //Tenant
        AbpTenantManagementDbProperties.DbTablePrefix = string.Empty;
        AbpTenantManagementDbProperties.DbSchema = DatabaseConstants.IdentitySchema;
        //Setting
        AbpSettingManagementDbProperties.DbTablePrefix = string.Empty;
        AbpSettingManagementDbProperties.DbSchema = DatabaseConstants.SettingSchema;
        //Permission
        AbpPermissionManagementDbProperties.DbTablePrefix = string.Empty;
        AbpPermissionManagementDbProperties.DbSchema = DatabaseConstants.IdentitySchema;
        //Open Iddict
        AbpOpenIddictDbProperties.DbTablePrefix = string.Empty;
        AbpOpenIddictDbProperties.DbSchema = DatabaseConstants.OpenIddictSchema;
        //Feature
        AbpFeatureManagementDbProperties.DbTablePrefix = string.Empty;
        AbpFeatureManagementDbProperties.DbSchema = DatabaseConstants.IdentitySchema;
    }
}
