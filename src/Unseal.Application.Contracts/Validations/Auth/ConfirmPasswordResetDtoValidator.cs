using FluentValidation;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Constants.Validations;
using Unseal.Dtos.Auth;
using Unseal.Localization;

namespace Unseal.Validations.Auth;

public class ConfirmPasswordResetDtoValidator : AbstractValidator<ConfirmPasswordResetDto>
{
    private readonly IStringLocalizer<UnsealResource> _stringLocalizer;

    public ConfirmPasswordResetDtoValidator(IStringLocalizer<UnsealResource> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;

        RuleFor(confirmPasswordResetDto => confirmPasswordResetDto.UserId)
            .NotEmpty()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Auth.ConfirmPasswordResetDto.UserIdIsRequired]);

        RuleFor(confirmPasswordResetDto => confirmPasswordResetDto.Token)
            .NotEmpty()
            .NotNull()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Auth.ConfirmPasswordResetDto.TokenIsRequired]);

        RuleFor(confirmPasswordResetDto => confirmPasswordResetDto.NewPassword)
            .NotEmpty()
            .NotNull()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Auth.ConfirmPasswordResetDto.NewPasswordIsRequired])
            .Matches(RegexConstants.PasswordRegexFormat)
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Auth.ConfirmPasswordResetDto.InvalidPasswordFormat]);
    }
}
