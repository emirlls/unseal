using Riok.Mapperly.Abstractions;
using Unseal.Dtos.Users;
using Unseal.Models.Users;  
using Volo.Abp.DependencyInjection;

namespace Unseal.Profiles.Users;

[Mapper]
public partial class UserMapper : ITransientDependency
{
    public partial GroupCreateModel MapGroupCreateDtoToModel(GroupCreateDto dto);
    
    [MapperIgnoreTarget(nameof(GroupCreateModel.StreamContent))]
    public partial GroupCreateModel MapGroupUpdateDtoToModel(GroupUpdateDto dto);
    
}