using FluentAssertions;
using PerfectApiTemplate.Application.Common.Models;
using PerfectApiTemplate.Application.Features.Customers.Commands;

namespace PerfectApiTemplate.Tests.Unit.Application;

public sealed class RequestResultTests
{
    [Fact]
    public void Success_Should_Set_Value()
    {
        var result = RequestResult<int>.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
        result.ErrorCode.Should().BeNull();
    }

    [Fact]
    public void ValidationFailure_Should_Set_Errors()
    {
        var errors = new Dictionary<string, string[]> { ["Name"] = new[] { "Required" } };

        var result = RequestResult<int>.ValidationFailure(errors);

        result.IsSuccess.Should().BeFalse();
        result.ValidationErrors.Should().ContainKey("Name");
        result.ErrorCode.Should().Be("validation_error");
    }
}

public sealed class CreateCustomerCommandValidatorTests
{
    private readonly CreateCustomerCommandValidator _validator = new();

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid()
    {
        var result = _validator.Validate(new CreateCustomerCommand("Test", "invalid", DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1))));

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Email");
    }

    [Fact]
    public void Should_Not_Have_Errors_When_Request_Is_Valid()
    {
        var result = _validator.Validate(new CreateCustomerCommand("Test", "test@example.com", DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-1))));

        result.IsValid.Should().BeTrue();
    }
}

