using System;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Entities.Capsules;
using Unseal.Extensions;
using Unseal.Interfaces.Managers.Capsules;
using Unseal.Localization;
using Unseal.Repositories.Base;

namespace Unseal.Managers.Capsules;

public class CapsuleMapFeatureManager : BaseDomainService<CapsuleMapFeature>, ICapsuleMapFeatureManager
{
    public CapsuleMapFeatureManager(
        IBaseRepository<CapsuleMapFeature> baseRepository,
        IStringLocalizer<UnsealResource> stringLocalizer
    ) : base(
        baseRepository,
        stringLocalizer,
        ExceptionCodes.CapsuleMapFeature.NotFound,
        ExceptionCodes.CapsuleMapFeature.AlreadyExists)
    {
    }

    public CapsuleMapFeature Create(Guid capsuleId, string geoJson)
    {
        var geom = geoJson.ToGeomFromGeoJson();
        var capsuleMapFeature = new CapsuleMapFeature(
            GuidGenerator.Create(),
            capsuleId,
            DateTime.Now
        )
        {
            Geom = geom
        };
        return capsuleMapFeature;
    }
}