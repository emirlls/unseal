using System;
using System.Collections.Generic;
using Riok.Mapperly.Abstractions;
using Unseal.Dtos.Users;
using Unseal.Entities.Users;
using Unseal.Extensions;
using Unseal.Models.Users;  
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

    public partial GroupCreateModel MapGroupCreateDtoToModel(GroupCreateDto dto);
    
    [MapperIgnoreTarget(nameof(GroupCreateModel.StreamContent))]
    public partial GroupCreateModel MapGroupUpdateDtoToModel(GroupUpdateDto dto);
    
    public partial List<GroupDto> MapGroupToGroupDtoList(List<Group> groups);

    public GroupDto MapCapsuleToCapsuleDto(Group group)
    {
        var decryptedFileUrl = _serviceProvider.GetDecryptedFileUrlAsync(group.GroupImageUrl!);
        var dto = new GroupDto(group.Id, group.Name, group.Description, decryptedFileUrl);
        return dto;
    }

}