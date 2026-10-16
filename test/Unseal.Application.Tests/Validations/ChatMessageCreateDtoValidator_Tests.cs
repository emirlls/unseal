using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Localization;
using NSubstitute;
using Shouldly;
using Unseal.Constants;
using Unseal.Constants.Validations;
using Unseal.Dtos.Messages;
using Unseal.Localization;
using Unseal.Validations.Messages;
using Xunit;

namespace Unseal.Validations;

public class ChatMessageCreateDtoValidator_Tests
{
    private readonly ChatMessageCreateDtoValidator _validator;

    public ChatMessageCreateDtoValidator_Tests()
    {
        var stringLocalizer = Substitute.For<IStringLocalizer<UnsealResource>>();
        stringLocalizer[Arg.Any<string>()]
            .Returns(callInfo => new LocalizedString((string)callInfo[0], (string)callInfo[0]));
        _validator = new ChatMessageCreateDtoValidator(stringLocalizer);
    }

    [Fact]
    public void Should_Accept_Valid_Message()
    {
        var result = _validator.Validate(CreateDto(new string('a', AppConstants.ChatMessage.MaxContentLength), 1));

        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Should_Reject_Too_Long_Content()
    {
        var result = _validator.Validate(CreateDto(new string('a', AppConstants.ChatMessage.MaxContentLength + 1), 1));

        result.Errors.ShouldContain(x =>
            x.ErrorMessage == ValidationErrorCodes.Messages.ChatMessageCreateDto.ContentTooLong);
    }

    [Fact]
    public void Should_Reject_Too_Many_Targets()
    {
        var result = _validator.Validate(CreateDto("hello", AppConstants.ChatMessage.MaxTargetCount + 1));

        result.Errors.ShouldContain(x =>
            x.ErrorMessage == ValidationErrorCodes.Messages.ChatMessageCreateDto.TooManyTargets);
    }

    private static ChatMessageCreateDto CreateDto(string content, int targetCount)
    {
        return new ChatMessageCreateDto
        {
            ChatTypeId = Guid.NewGuid(),
            Content = content,
            TargetIds = Enumerable.Range(0, targetCount).Select(_ => Guid.NewGuid()).ToList()
        };
    }
}
