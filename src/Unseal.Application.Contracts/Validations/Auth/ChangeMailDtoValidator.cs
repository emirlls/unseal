using FluentValidation;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Constants.Validations;
using Unseal.Dtos.Auth;
using Unseal.Localization;

namespace Unseal.Validations.Auth;

public class ChangeMailDtoValidator : AbstractValidator<ChangeMailDto>
{
    private readonly IStringLocalizer<UnsealResource> _stringLocalizer;

    public ChangeMailDtoValidator(IStringLocalizer<UnsealResource> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;

        RuleFor(changeMailDto => changeMailDto.NewMailAddress)
            .NotEmpty()
            .NotNull()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Auth.ChangeMailDto.NewMailAddressIsRequired])
            .Matches(RegexConstants.MailRegexFormat)
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Auth.ChangeMailDto.InvalidEmailFormat]);

        RuleFor(changeMailDto => changeMailDto.Password)
            .NotEmpty()
            .NotNull()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Auth.ChangeMailDto.PasswordIsRequired]);
    }
}
