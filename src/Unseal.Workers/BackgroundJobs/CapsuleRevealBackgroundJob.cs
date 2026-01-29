using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Unseal.Constants;
using Unseal.Filtering.Base;
using Unseal.Filtering.Capsules;
using Unseal.Repositories.Capsules;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Unseal.Workers.BackgroundJobs;

public class CapsuleRevealBackgroundJob : ITransientDependency
{
    public async Task RevealCapsulesAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    )
    {
        var tenantDataFilter = serviceProvider.GetRequiredService<IDataFilter<IMultiTenant>>();
        using (tenantDataFilter.Disable())
        {
            var capsuleRepository = serviceProvider.GetRequiredService<ICapsuleRepository>();
            var now = DateTime.Now;
            var capsuleFilters = new CapsuleFilters
            {
                Filters = new List<FilterItem>()
                {
                    new FilterItem
                    {
                        Prop = CapsuleFilters.GetIsOpened(),
                        Strategy = FilterOperators.Equals,
                        Value = false.ToString()
                    },
                    new FilterItem
                    {
                        Prop = CapsuleFilters.GetRevealDate(),
                        Strategy = FilterOperators.LessThanOrEqual,
                        Value = now.AddMinutes(-5).ToString(CultureInfo.InvariantCulture)
                    }
                }
            };
            var notOpenedCapsules = await capsuleRepository
                .GetDynamicListAsync(
                    capsuleFilters,
                    null,
                    false,
                    cancellationToken
                );
            await Parallel.ForEachAsync(notOpenedCapsules, cancellationToken,
                async (capsule, ct) => { capsule.IsOpened = true; });
            await capsuleRepository.BulkUpdateAsync(notOpenedCapsules, cancellationToken);
        }
    }
}