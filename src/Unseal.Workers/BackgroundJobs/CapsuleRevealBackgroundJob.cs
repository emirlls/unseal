using Volo.Abp.DependencyInjection;

namespace Unseal.Workers.BackgroundJobs;

public class CapsuleRevealBackgroundJob : ITransientDependency
{
    public async Task RevealCapsulesAsync(CancellationToken cancellationToken = default)
    {
        
    }
}