using FluentValidation;
using Microsoft.Extensions.Localization;
using Unseal.Constants;
using Unseal.Constants.Validations;
using Unseal.Dtos.Messages;
using Unseal.Localization;

namespace Unseal.Validations.Messages;

public class ChatMessageCreateDtoValidator : AbstractValidator<ChatMessageCreateDto>
{
    private readonly IStringLocalizer<UnsealResource> _stringLocalizer;

    public ChatMessageCreateDtoValidator(IStringLocalizer<UnsealResource> stringLocalizer)
    {
        _stringLocalizer = stringLocalizer;

        RuleFor(group => group.TargetIds)
            .NotNull()
            .NotEmpty()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Messages.ChatMessageCreateDto.TargetIsRequired])
            .Must(targetIds => targetIds == null || targetIds.Count <= AppConstants.ChatMessage.MaxTargetCount)
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Messages.ChatMessageCreateDto.TooManyTargets]);

        RuleFor(group => group.ChatTypeId)
            .NotNull()
            .NotEmpty()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Messages.ChatMessageCreateDto.ChatTypeIsRequired]);

        RuleFor(group => group.Content)
            .NotNull()
            .NotEmpty()
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Messages.ChatMessageCreateDto.ContentIsRequired])
            .MaximumLength(AppConstants.ChatMessage.MaxContentLength)
            .WithMessage(_stringLocalizer[ValidationErrorCodes.Messages.ChatMessageCreateDto.ContentTooLong]);
    }
}