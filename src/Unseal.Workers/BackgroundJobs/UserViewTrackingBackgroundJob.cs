using Microsoft.Extensions.DependencyInjection;
using Unseal.Repositories.Users;
using Volo.Abp.DependencyInjection;

namespace Unseal.Workers.BackgroundJobs;

public class UserViewTrackingBackgroundJob : ITransientDependency
{
    public async Task CleanUserViewTrackingAsync(
        IServiceProvider serviceProvider,
        CancellationToken workerContextCancellationToken = default
    )
    {
        var userViewTrackingRepository = serviceProvider.GetRequiredService<IUserViewTrackingRepository>();
        var dataInLastMonth = await userViewTrackingRepository
            .GetListByAsync(x => x.CreationTime <= DateTime.Now.AddMonths(-1),
                cancellationToken: workerContextCancellationToken);
        
        if (!dataInLastMonth.IsNullOrEmpty())
        {
            await userViewTrackingRepository.HardDeleteManyAsync(dataInLastMonth!, workerContextCancellationToken);
        }
    }
}