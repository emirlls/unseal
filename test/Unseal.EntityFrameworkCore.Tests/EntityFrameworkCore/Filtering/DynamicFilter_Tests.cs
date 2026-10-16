using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Unseal.Constants;
using Unseal.Entities.Capsules;
using Unseal.Extensions;
using Unseal.Filtering.Base;
using Unseal.Filtering.Capsules;
using Volo.Abp;
using Xunit;

namespace Unseal.EntityFrameworkCore.Filtering;

public class DynamicFilter_Tests
{
    private readonly List<Capsule> _capsules = new()
    {
        CreateCapsule("alpha"),
        CreateCapsule("beta"),
        CreateCapsule("gamma")
    };

    [Theory]
    [InlineData("CapsuleItems.FileUrl")]
    [InlineData("ReceiverId")]
    [InlineData("Sorting")]
    [InlineData("UnknownProperty")]
    public void Should_Reject_Properties_Without_FilterMapped(string prop)
    {
        var filters = CreateFilters(prop, FilterOperators.Equals, "value");

        var exception = Should.Throw<UserFriendlyException>(() =>
            _capsules.AsQueryable().ApplyDynamicFilters(filters).ToList());

        exception.Code.ShouldBe(ExceptionCodes.Filter.InvalidProperty);
    }

    [Theory]
    [InlineData("~")]
    [InlineData("like")]
    public void Should_Reject_Unknown_Strategies(string strategy)
    {
        var filters = CreateFilters("Name", strategy, "alpha");

        var exception = Should.Throw<UserFriendlyException>(() =>
            _capsules.AsQueryable().ApplyDynamicFilters(filters).ToList());

        exception.Code.ShouldBe(ExceptionCodes.Filter.InvalidStrategy);
    }

    [Fact]
    public void Should_Reject_Strategy_Not_Applicable_To_Field_Type()
    {
        var filters = CreateFilters("CreatorId", FilterOperators.Contains, Guid.NewGuid().ToString());

        var exception = Should.Throw<UserFriendlyException>(() =>
            _capsules.AsQueryable().ApplyDynamicFilters(filters).ToList());

        exception.Code.ShouldBe(ExceptionCodes.Filter.InvalidStrategy);
    }

    [Theory]
    [InlineData("CreatorId", "not-a-guid")]
    [InlineData("RevealDate", "not-a-date")]
    public void Should_Reject_Invalid_Values(string prop, string value)
    {
        var filters = CreateFilters(prop, FilterOperators.Equals, value);

        var exception = Should.Throw<UserFriendlyException>(() =>
            _capsules.AsQueryable().ApplyDynamicFilters(filters).ToList());

        exception.Code.ShouldBe(ExceptionCodes.Filter.InvalidValue);
    }

    [Fact]
    public void Should_Apply_FilterMapped_Properties()
    {
        var filters = CreateFilters("name", FilterOperators.Equals, "beta");

        var result = _capsules.AsQueryable().ApplyDynamicFilters(filters).ToList();

        result.ShouldHaveSingleItem().Name.ShouldBe("beta");
    }

    [Fact]
    public void Should_Apply_Server_Side_Filters()
    {
        var publicTypeId = Guid.Parse(LookupSeederConstants.CapsuleTypesConstants.Public.Id);
        var filters = new CapsuleFilters();
        filters.SetIsOpened(true);
        filters.SetIsActive(true);
        filters.SetIsPublic(publicTypeId);
        _capsules[0].IsOpened = true;
        _capsules[0].IsActive = true;
        _capsules[0].CapsuleTypeId = publicTypeId;

        var result = _capsules.AsQueryable().ApplyDynamicFilters(filters).ToList();

        result.ShouldHaveSingleItem().Name.ShouldBe("alpha");
    }

    [Theory]
    [InlineData("ReceiverId")]
    [InlineData("SkipCount")]
    [InlineData("Name sideways")]
    [InlineData("Name desc extra")]
    public void Should_Reject_Invalid_Sorting(string sorting)
    {
        var exception = Should.Throw<UserFriendlyException>(() =>
            SortStrategyExecutor.ApplySorting<Capsule, CapsuleFilters>(_capsules.AsQueryable(), sorting).ToList());

        exception.Code.ShouldBe(ExceptionCodes.Filter.InvalidSorting);
    }

    [Fact]
    public void Should_Apply_FilterMapped_Sorting()
    {
        var result = SortStrategyExecutor
            .ApplySorting<Capsule, CapsuleFilters>(_capsules.AsQueryable(), "Name desc")
            .Select(x => x.Name)
            .ToList();

        result.ShouldBe(new[] { "gamma", "beta", "alpha" });
    }

    [Theory]
    [InlineData(1000, 100)]
    [InlineData(0, 1)]
    [InlineData(-5, 1)]
    [InlineData(25, 25)]
    public void MaxResultCount_Should_Be_Capped(int requested, int expected)
    {
        var filters = new CapsuleFilters { MaxResultCount = requested };

        filters.MaxResultCount.ShouldBe(expected);
    }

    [Fact]
    public void SkipCount_Should_Not_Be_Negative()
    {
        var filters = new CapsuleFilters { SkipCount = -10 };

        filters.SkipCount.ShouldBe(0);
    }

    private static CapsuleFilters CreateFilters(string prop, string strategy, string value)
    {
        return new CapsuleFilters
        {
            Filters = new List<FilterItem>
            {
                new() { Prop = prop, Strategy = strategy, Value = value }
            }
        };
    }

    private static Capsule CreateCapsule(string name)
    {
        return new Capsule(
            Guid.NewGuid(),
            null,
            null,
            null,
            Guid.NewGuid(),
            name,
            DateTime.Now);
    }
}
