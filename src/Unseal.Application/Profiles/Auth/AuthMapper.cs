using Riok.Mapperly.Abstractions;
using Unseal.Dtos.Auth;
using Unseal.Models.Auth;
using Volo.Abp.DependencyInjection;

namespace Unseal.Profiles.Auth;

[Mapper]
public partial class AuthMapper : ITransientDependency
{
    public partial RegisterModel MapToModel(RegisterDto dto);
}