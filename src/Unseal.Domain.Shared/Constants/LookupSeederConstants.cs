using Unseal.Enums;

namespace Unseal.Constants;

public class LookupSeederConstants
{
    public static class CapsuleTypesConstants
    {
        public static readonly CapsuleTypeInfo Personal = new(
            "0b2374ec-0040-4d5d-b9a6-5c6333b458c8",
            "Personal",
            (int)(CapsuleTypes.Personal));

        public static readonly CapsuleTypeInfo DirectMessage = new(
            "6e859f2b-3b3c-44df-9a16-a0c2d5bfc51e",
            "DirectMessage",
            (int)(CapsuleTypes.DirectMessage));

        public static readonly CapsuleTypeInfo PublicBroadcast = new(
            "fd4f8d92-deb2-493d-a38c-5509b4ad7382",
            "PublicBroadcast",
            (int)CapsuleTypes.PublicBroadcast);

        public static readonly CapsuleTypeInfo Collaborative = new(
            "8bd05f2f-5abc-4e58-aa1b-bd7d7419d243",
            "Collaborative",
            (int)CapsuleTypes.Collaborative);

        public static readonly CapsuleTypeInfo Public = new(
            "34b8f36b-8f43-449a-a86b-011ceb8c7f5b",
            "Public",
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
            "UserRegister",
            (int)(NotificationEventTypes.UserRegister));

        public static readonly NotificationEventTypeInfo UserDelete = new(
            "d5961c50-522f-4bea-a641-ab88ae983985",
            "UserDelete",
            (int)(NotificationEventTypes.UserDelete));
        
        public static readonly NotificationEventTypeInfo ConfirmChangeMail = new(
            "b91d42b6-fae7-4376-a2e5-8cdca52402a0",
            "ConfirmChangeMail",
            (int)(NotificationEventTypes.ConfirmChangeMail));
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
}