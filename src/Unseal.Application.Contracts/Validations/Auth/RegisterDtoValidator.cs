using FluentValidation;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Constants.Validations;
using Unseal.Dtos.Auth;
using Unseal.Localization;

namespace Unseal.Validations.Auth;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    private readonly IStringLocalizer<UnsealResource> _stringLocalizer;

    public RegisterDtoValidator(IStringLocalizer<UnsealResource> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        RuleFor(dto => dto.Email)
            .NotEmpty()
            .NotNull()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Auth.RegisterDto.EmailIsRequired])
            .Matches(RegexConstants.MailRegexFormat)
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Auth.RegisterDto.InvalidEmailFormat]);

        RuleFor(dto => dto.Username)
            .NotEmpty()
            .NotNull()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Auth.RegisterDto.UsernameIsRequired]);

        RuleFor(dto => dto.Password)
            .NotEmpty()
            .NotNull()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Auth.RegisterDto.PasswordIsRequired])
            .Matches(RegexConstants.PasswordRegexFormat)
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Auth.RegisterDto.InvalidPasswordFormat]);
        
        RuleFor(dto => dto.ConfirmPassword)
            .NotEmpty()
            .NotNull()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Auth.RegisterDto.ConfirmPasswordIsRequired])
            .Matches(RegexConstants.PasswordRegexFormat)
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Auth.RegisterDto.InvalidPasswordFormat])
            .Equal(dto => dto.Password)
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Auth.RegisterDto.PasswordsDontMatch]);

        RuleFor(registerDto => registerDto.FirstName)
            .NotEmpty()
            .NotNull()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Auth.RegisterDto.FirstNameIsRequired]);

        RuleFor(registerDto => registerDto.LastName)
            .NotEmpty()
            .NotNull()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Auth.RegisterDto.LastNameIsRequired]);
        
    }
}