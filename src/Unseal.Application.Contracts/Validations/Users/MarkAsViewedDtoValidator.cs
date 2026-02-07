using FluentValidation;
using Microsoft.Extensions.Localization;
using Unseal.Constants.Validations;
using Unseal.Dtos.Users;
using Unseal.Localization;

namespace Unseal.Validations.Users;

public class MarkAsViewedDtoValidator : AbstractValidator<MarkAsViewedDto>
{
    private readonly IStringLocalizer<UnsealResource> _stringLocalizer;

    public MarkAsViewedDtoValidator(IStringLocalizer<UnsealResource> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;

        RuleFor(dto => dto.UserViewTrackingTypeId)
            .NotNull()
            .NotEmpty()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Users.MarkAsViewedDto.TypeIsRequired]);
        
        RuleFor(dto => dto.ExternalIds)
            .NotNull()
            .NotEmpty()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Users.UserProfileUpdateDto.AllowJoinGroupIsRequired]);
    }
}