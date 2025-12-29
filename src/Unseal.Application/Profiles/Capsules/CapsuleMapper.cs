using Riok.Mapperly.Abstractions;
using Unseal.Dtos.Capsules;
using Unseal.Models.Capsules;
using Volo.Abp.DependencyInjection;

namespace Unseal.Profiles.Capsules;

[Mapper]
public partial class CapsuleMapper : ITransientDependency
{
    public CapsuleCreateModel MapCapsuleCreateDtoToCapsuleCreateModel(CapsuleCreateDto capsuleCreateDto)
    {
        return new CapsuleCreateModel(
            capsuleCreateDto.CapsuleTypeId,
            capsuleCreateDto.ReceiverId,
            capsuleCreateDto.Name,
            capsuleCreateDto.IsPublic,
            capsuleCreateDto.StreamContent.ContentType,
            capsuleCreateDto.TextContext,
            string.Empty, // will be set after upload file.
            capsuleCreateDto.StreamContent.FileName,
            capsuleCreateDto.RevealDate);
    }
}