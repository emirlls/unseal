using System;
using Microsoft.EntityFrameworkCore;
using Unseal.Constants;
using Unseal.Entities.Lookups;

namespace Unseal.Extensions;

public static class LookupSeeders
{
    public static void LookupSeeder(this ModelBuilder builder)
    {
        #region SoilTypes

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
    }
}