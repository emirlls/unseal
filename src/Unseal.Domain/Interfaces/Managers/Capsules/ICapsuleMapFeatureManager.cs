using System;
using Unseal.Entities.Capsules;

namespace Unseal.Interfaces.Managers.Capsules;

public interface ICapsuleMapFeatureManager : IBaseDomainService<CapsuleMapFeature>
{
    CapsuleMapFeature Create(Guid capsuleId, string geoJson);
}