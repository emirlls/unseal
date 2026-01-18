using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using Unseal.Constants;
using Unseal.Filtering.Base;
using Unseal.Filtering.Capsules;
using Unseal.Repositories.Capsules;
using Volo.Abp.DependencyInjection;

namespace Unseal.Workers.BackgroundJobs;

public class CapsuleRevealBackgroundJob : ITransientDependency
{
    public async Task RevealCapsulesAsync(
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    )
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
                    Strategy = FilterOperators.GreaterThanOrEqual,
                    Value = now.AddMinutes(-5).ToString(CultureInfo.InvariantCulture)
                }
            }
        };
        var notOpenedCapsules = await capsuleRepository
            .GetDynamicListAsync(
                capsuleFilters,
                null,
                true,
                cancellationToken
            );
        await Parallel.ForEachAsync(notOpenedCapsules, cancellationToken,
            async (capsule, ct) =>
        {
            capsule.IsOpened = true;
        });
        await capsuleRepository.BulkUpdateAsync(notOpenedCapsules, cancellationToken);
    }
}