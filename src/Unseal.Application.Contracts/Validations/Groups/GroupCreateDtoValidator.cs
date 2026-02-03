using FluentValidation;
using Microsoft.Extensions.Localization;
using Unseal.Constants.Validations;
using Unseal.Dtos.Groups;
using Unseal.Localization;

namespace Unseal.Validations.Groups;

public class GroupCreateDtoValidator : AbstractValidator<GroupCreateDto>
{
    private readonly IStringLocalizer<UnsealResource> _stringLocalizer;

    public GroupCreateDtoValidator(IStringLocalizer<UnsealResource> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        
        RuleFor(dto => dto.UserIds)
            .NotNull()
            .NotEmpty()
            .WithMessage(stringLocalizer[ValidationErrorCodes.Groups.GroupCreateDto.UsersIsRequired]);
        RuleFor(x => x.Name)
            .NotNull()
            .NotEmpty()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Groups.GroupCreateDto.NameIsRequired]);
    }
}