using System;
using Riok.Mapperly.Abstractions;
using Volo.Abp.DependencyInjection;

namespace Unseal.Profiles.Users;

[Mapper]
public partial class UserMapper : ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public UserMapper(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

}