using Unseal.Enums;

namespace Unseal.Constants;

public static class LookupSeederConstants
{
    public static class CapsuleTypesConstants
    {
        public static readonly CapsuleTypeInfo Personal = new(
            "0b2374ec-0040-4d5d-b9a6-5c6333b458c8",
            nameof(Personal),
            (int)(CapsuleTypes.Personal));
        
        public static readonly CapsuleTypeInfo Public = new(
            "34b8f36b-8f43-449a-a86b-011ceb8c7f5b",
            nameof(Public),
            (int)CapsuleTypes.Public);
    }
    
    public class CapsuleTypeInfo
    {
        public string Id { get; }
        public string Name { get; }
        public int Code { get; }

        public CapsuleTypeInfo(string id, string name, int code)
        {
            Id = id;
            Name = name;
            Code = code;
        }
    }
    
    public static class NotificationEventTypesConstants
    {
        public static readonly NotificationEventTypeInfo UserRegister = new(
            "a9745b1b-3aa1-4a57-b5a6-8a4e07fe778b",
            nameof(UserRegister),
            (int)(NotificationEventTypes.UserRegister));

        public static readonly NotificationEventTypeInfo UserDelete = new(
            "d5961c50-522f-4bea-a641-ab88ae983985",
            nameof(UserDelete),
            (int)(NotificationEventTypes.UserDelete));
        
        public static readonly NotificationEventTypeInfo ConfirmChangeMail = new(
            "b91d42b6-fae7-4376-a2e5-8cdca52402a0",
            nameof(ConfirmChangeMail),
            (int)(NotificationEventTypes.ConfirmChangeMail));
        
        public static readonly NotificationEventTypeInfo UserActivation = new(
            "5d5650d6-b2ce-42d1-9f00-a4eea225d81e",
            nameof(UserActivation),
            (int)(NotificationEventTypes.UserActivation));
    }
    
    public class NotificationEventTypeInfo
    {
        public string Id { get; }
        public string Name { get; }
        public int Code { get; }

        public NotificationEventTypeInfo(string id, string name, int code)
        {
            Id = id;
            Name = name;
            Code = code;
        }
    }
    
    public static class ChatTypesConstants
    {
        public static readonly ChatTypeInfo Directly = new(
            "b15c886a-2929-43d2-ba06-416683a19420",
            nameof(Directly),
            (int)(ChatTypes.Directly));

        public static readonly ChatTypeInfo Group = new(
            "121b1120-d7f8-41f9-bc06-32d72811d989",
            nameof(Group),
            (int)(ChatTypes.Group));
    }
    
    public class ChatTypeInfo
    {
        public string Id { get; }
        public string Name { get; }
        public int Code { get; }

        public ChatTypeInfo(string id, string name, int code)
        {
            Id = id;
            Name = name;
            Code = code;
        }
    }
    
    public static class UserFollowStatusesConstants
    {
        public static readonly UserFollowStatusInfo Pending = new(
            "e96877f3-5a69-4610-ac3b-7b224a1c17a9",
            nameof(Pending),
            (int)(UserFollowStatuses.Pending));

        public static readonly UserFollowStatusInfo Accepted = new(
            "585e849d-42aa-4ec7-a581-a56a7da434d8",
            nameof(Accepted),
            (int)(UserFollowStatuses.Accepted));
        
        public static readonly UserFollowStatusInfo Rejected = new(
            "9badcd74-212f-4e7e-9d04-5d0f2c28b713",
            nameof(Rejected),
            (int)(UserFollowStatuses.Rejected));
    }
    
    public class UserFollowStatusInfo
    {
        public string Id { get; }
        public string Name { get; }
        public int Code { get; }

        public UserFollowStatusInfo(string id, string name, int code)
        {
            Id = id;
            Name = name;
            Code = code;
        }
    }
    
    public static class UserViewTrackingTypesConstants
    {
        public static readonly UserViewTrackingTypeInfo Capsule = new(
            "0a08cad2-e210-43a7-99ce-513a752d422e",
            nameof(Capsule),
            (int)(UserViewTrackingTypes.Capsule));
        
        public static readonly UserViewTrackingTypeInfo UserProfile = new(
            "4ac149d8-d61a-4efe-9a6c-81c6c0520fba",
            nameof(UserProfile),
            (int)UserViewTrackingTypes.UserProfile);
    }
    
    public class UserViewTrackingTypeInfo
    {
        public string Id { get; }
        public string Name { get; }
        public int Code { get; }

        public UserViewTrackingTypeInfo(string id, string name, int code)
        {
            Id = id;
            Name = name;
            Code = code;
        }
    }
}