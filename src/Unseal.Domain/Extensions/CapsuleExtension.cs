using System;
using System.Linq;
using System.Linq.Expressions;
using Unseal.Constants;
using Unseal.Entities.Capsules;

namespace Unseal.Extensions;

public static class CapsuleExtension
{
    private static readonly Guid PublicCapsuleTypeId =
        Guid.Parse(LookupSeederConstants.CapsuleTypesConstants.Public.Id);

    /// <summary>
    /// The creator can always see a capsule. Other users can see it only after it is opened and while it is
    /// active, and only if it is public or they are its receiver.
    /// </summary>
    public static Expression<Func<Capsule, bool>> IsVisibleTo(Guid userId)
    {
        return x => x.CreatorId == userId ||
                    (x.IsOpened == true &&
                     x.IsActive == true &&
                     (x.CapsuleTypeId == PublicCapsuleTypeId || x.ReceiverId == userId));
    }

    public static IQueryable<Capsule> WhereVisibleTo(this IQueryable<Capsule> query, Guid userId)
    {
        return query.Where(IsVisibleTo(userId));
    }
}
