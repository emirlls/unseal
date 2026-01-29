using FluentValidation;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Constants.Validations;
using Unseal.Dtos.Auth;
using Unseal.Localization;

namespace Unseal.Validations.Auth;

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    private readonly IStringLocalizer<UnsealResource> _stringLocalizer;

    public ChangePasswordDtoValidator(IStringLocalizer<UnsealResource> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        
        RuleFor(changePasswordDto => changePasswordDto.OldPassword)
            .NotEmpty()
            .NotNull()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Auth.ChangePasswordDto.OldPasswordIsRequired]);

        RuleFor(changePasswordDto => changePasswordDto.NewPassword)
            .NotEmpty()
            .NotNull()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Auth.ChangePasswordDto.NewPasswordIsRequired])
            .Matches(RegexConstants.PasswordRegexFormat)
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Auth.ChangePasswordDto.InvalidPasswordFormat]);
    }
}