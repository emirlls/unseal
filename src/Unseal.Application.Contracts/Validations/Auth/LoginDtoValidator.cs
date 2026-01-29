using FluentValidation;
using Microsoft.Extensions.Localization;
using Unseal.Constants.Validations;
using Unseal.Dtos.Auth;
using Unseal.Localization;

namespace Unseal.Validations.Auth;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    private readonly IStringLocalizer<UnsealResource> _stringLocalizer;

    public LoginDtoValidator(IStringLocalizer<UnsealResource> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        
        RuleFor(loginDto => loginDto.Email)
            .NotEmpty()
            .NotNull()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Auth.LoginDto.EmailIsRequired]);
        
        RuleFor(loginDto => loginDto.Password)
            .NotEmpty()
            .NotNull()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Auth.LoginDto.PasswordIsRequired]);
    }
}