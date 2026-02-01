using FluentAssertions;
using PerfectApiTemplate.Application.Features.Emails.Commands.EnqueueEmail;

namespace PerfectApiTemplate.Tests.Unit.Application;

public sealed class EmailValidatorTests
{
    [Fact]
    public void EnqueueEmail_Should_Validate_Required_Fields()
    {
        var validator = new EnqueueEmailCommandValidator();
        var result = validator.Validate(new EnqueueEmailCommand(null, "", "", "", false));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "To");
        result.Errors.Should().Contain(x => x.PropertyName == "Subject");
        result.Errors.Should().Contain(x => x.PropertyName == "Body");
    }

    [Fact]
    public void EnqueueEmail_Should_Accept_Valid_Payload()
    {
        var validator = new EnqueueEmailCommandValidator();
        var result = validator.Validate(new EnqueueEmailCommand("sender@test.local", "recipient@test.local", "Hi", "Body", false));

        result.IsValid.Should().BeTrue();
    }
}

