using System;
using FluentValidation;
using Microsoft.Extensions.Localization;
using Unseal.Constants.Validations;
using Unseal.Dtos.Capsules;
using Unseal.Localization;

namespace Unseal.Validations.Capsules;

public class CapsuleCreateDtoValidator : AbstractValidator<CapsuleCreateDto>
{
    private readonly IStringLocalizer<UnsealResource> _stringLocalizer;

    public CapsuleCreateDtoValidator(IStringLocalizer<UnsealResource> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;
        
        RuleFor(dto => dto.Name)
            .NotEmpty()
            .NotNull()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Capsules.CapsuleCreateDto.NameIsRequired]);
        
        RuleFor(dto => dto.RevealDate)
            .NotEmpty()
            .NotNull()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Capsules.CapsuleCreateDto.RevealDateIsRequired])
            .Must(CheckValidDate)
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Capsules.CapsuleCreateDto.RevealDateIsRequired])
            .GreaterThan(DateTime.UtcNow)
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Capsules.CapsuleCreateDto.RevealDateMustBeInFuture]);
    }
    private bool CheckValidDate(DateTime date)
    {
        return date != default;
    }
}