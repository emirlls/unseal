using Microsoft.Extensions.DependencyInjection;
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
            IsOpened = false,
            RevealDateMin = now.AddMinutes(-5),
            RevealDateMax = now
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