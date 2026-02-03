using System;
using System.Collections.Generic;
using System.Linq;
using Riok.Mapperly.Abstractions;
using Unseal.Dtos.Groups;
using Unseal.Entities.Groups;
using Unseal.Extensions;
using Unseal.Models.Groups;
using Volo.Abp.DependencyInjection;

namespace Unseal.Profiles.Groups;
[Mapper]
public partial class GroupMapper : ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public GroupMapper(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public partial GroupCreateModel MapToModel(GroupCreateDto dto);
    
    [MapperIgnoreTarget(nameof(GroupCreateModel.StreamContent))]
    public partial GroupCreateModel MapToModel(GroupUpdateDto dto);

    public List<GroupDto> MapToDto(List<Group> groups)
    {
        return groups.Select(MapToDto).ToList();
    }

    private GroupDto MapToDto(Group group)
    {
        var decryptedFileUrl = _serviceProvider.GetDecryptedFileUrlAsync(group.GroupImageUrl!);
        var dto = new GroupDto(group.Id, group.Name, group.Description, decryptedFileUrl);
        return dto;
    }
    
    public GroupDetailDto MapToDetailDto(Group group)
    {
        var decryptedFileUrl = _serviceProvider.GetDecryptedFileUrlAsync(group.GroupImageUrl!);
        var members = GetGroupMembers(group);
        var dto = new GroupDetailDto(group.Id, group.Name, group.Description, decryptedFileUrl,members);
        return dto;
    }

    private List<GroupMemberDto> GetGroupMembers(Group group)
    {
        var members = group.GroupMembers
            .Select(x => new GroupMemberDto(x.UserId, x.User!.Name, x.User.Surname))
            .ToList();
        return members;
    }
}