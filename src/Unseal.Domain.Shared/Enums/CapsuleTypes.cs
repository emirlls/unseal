using System.ComponentModel;

namespace Unseal.Enums;

public enum CapsuleTypes
{
    [Description("CapsuleTypes:00")]
    Personal = 0,       // Gelecekteki kendime
    [Description("CapsuleTypes:01")]
    DirectMessage = 1,  // Bir arkadaşıma özel
    [Description("CapsuleTypes:02")]
    PublicBroadcast = 2,// Tüm takipçilerime
    [Description("CapsuleTypes:03")]
    Collaborative = 3,   // Ortak (Grup) kapsülü
    [Description("CapsuleTypes:04")]
    Public = 4   // Herkese
}