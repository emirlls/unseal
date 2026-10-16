using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Unseal.Constants;
using Unseal.Entities.Capsules;
using Unseal.Extensions;
using Xunit;

namespace Unseal.Capsules;

public class CapsuleVisibility_Tests
{
    private static readonly Guid PublicTypeId = Guid.Parse(LookupSeederConstants.CapsuleTypesConstants.Public.Id);
    private static readonly Guid PersonalTypeId = Guid.Parse(LookupSeederConstants.CapsuleTypesConstants.Personal.Id);

    private readonly Guid _creatorId = Guid.NewGuid();
    private readonly Guid _receiverId = Guid.NewGuid();
    private readonly Guid _strangerId = Guid.NewGuid();

    [Fact]
    public void Creator_Should_See_Own_Sealed_Capsule()
    {
        var capsule = CreateCapsule(PersonalTypeId, isOpened: false, isActive: true);

        IsVisible(capsule, _creatorId).ShouldBeTrue();
    }

    [Fact]
    public void Creator_Should_See_Own_Archived_Capsule()
    {
        var capsule = CreateCapsule(PublicTypeId, isOpened: true, isActive: false);

        IsVisible(capsule, _creatorId).ShouldBeTrue();
    }

    [Fact]
    public void Others_Should_See_Opened_Active_Public_Capsule()
    {
        var capsule = CreateCapsule(PublicTypeId, isOpened: true, isActive: true);

        IsVisible(capsule, _strangerId).ShouldBeTrue();
    }

    [Fact]
    public void Others_Should_Not_See_Sealed_Public_Capsule()
    {
        var capsule = CreateCapsule(PublicTypeId, isOpened: false, isActive: true);

        IsVisible(capsule, _strangerId).ShouldBeFalse();
    }

    [Fact]
    public void Others_Should_Not_See_Archived_Public_Capsule()
    {
        var capsule = CreateCapsule(PublicTypeId, isOpened: true, isActive: false);

        IsVisible(capsule, _strangerId).ShouldBeFalse();
    }

    [Fact]
    public void Others_Should_Not_See_Opened_Personal_Capsule()
    {
        var capsule = CreateCapsule(PersonalTypeId, isOpened: true, isActive: true);

        IsVisible(capsule, _strangerId).ShouldBeFalse();
    }

    [Fact]
    public void Receiver_Should_See_Opened_Personal_Capsule()
    {
        var capsule = CreateCapsule(PersonalTypeId, isOpened: true, isActive: true);

        IsVisible(capsule, _receiverId).ShouldBeTrue();
    }

    [Fact]
    public void Receiver_Should_Not_See_Sealed_Personal_Capsule()
    {
        var capsule = CreateCapsule(PersonalTypeId, isOpened: false, isActive: true);

        IsVisible(capsule, _receiverId).ShouldBeFalse();
    }

    [Fact]
    public void WhereVisibleTo_Should_Filter_Query()
    {
        var visible = CreateCapsule(PublicTypeId, isOpened: true, isActive: true);
        var sealedCapsule = CreateCapsule(PublicTypeId, isOpened: false, isActive: true);
        var personal = CreateCapsule(PersonalTypeId, isOpened: true, isActive: true);

        var result = new List<Capsule> { visible, sealedCapsule, personal }
            .AsQueryable()
            .WhereVisibleTo(_strangerId)
            .ToList();

        result.ShouldHaveSingleItem().Id.ShouldBe(visible.Id);
    }

    private Capsule CreateCapsule(Guid capsuleTypeId, bool isOpened, bool isActive)
    {
        return new Capsule(
            Guid.NewGuid(),
            null,
            _receiverId,
            capsuleTypeId,
            _creatorId,
            "capsule",
            DateTime.Now)
        {
            IsOpened = isOpened,
            IsActive = isActive
        };
    }

    private static bool IsVisible(Capsule capsule, Guid userId)
    {
        return CapsuleExtension.IsVisibleTo(userId).Compile()(capsule);
    }
}
