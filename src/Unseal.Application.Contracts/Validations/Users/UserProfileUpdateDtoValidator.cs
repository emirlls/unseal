using FluentValidation;
using Microsoft.Extensions.Localization;
using Unseal.Constants.Validations;
using Unseal.Dtos.Users;
using Unseal.Localization;

namespace Unseal.Validations.Users;

public class UserProfileUpdateDtoValidator : AbstractValidator<UserProfileUpdateDto>
{
    private readonly IStringLocalizer<UnsealResource> _stringLocalizer;

    public UserProfileUpdateDtoValidator(IStringLocalizer<UnsealResource> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;

        RuleFor(dto => dto.IsLocked)
            .NotNull()
            .NotEmpty()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Users.UserProfileUpdateDto.IsLockedIsRequired]);
        
        RuleFor(dto => dto.AllowJoinGroup)
            .NotNull()
            .NotEmpty()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Users.UserProfileUpdateDto.AllowJoinGroupIsRequired]);
    }
}